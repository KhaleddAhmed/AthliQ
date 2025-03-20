﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AthliQ.Core;
using AthliQ.Core.DTOs.Child;
using AthliQ.Core.DTOs.Sport;
using AthliQ.Core.Entities.Models;
using AthliQ.Core.Responses;
using AthliQ.Core.Service.Contract;
using AthliQ.Service.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AthliQ.Service.Services.Children
{
	public class ChildService : IChildService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly HttpClient _httpClient;

		public ChildService(IUnitOfWork unitOfWork, IMapper mapper, HttpClient httpClient)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_httpClient = httpClient;
		}

		public async Task<GenericResponse<bool>> CreateChildAsync(
			string userId,
			CreateChildDto createChildDto
		)
		{
			var genericResponse = new GenericResponse<bool>();

			// Case 01: If Any of entered Data is Invalid
			if (createChildDto is null)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Enter Valid Data";
				return genericResponse;
			}

			// Validate Gender
			if (
				createChildDto.Gender.ToLower() != "male"
				&& createChildDto.Gender.ToLower() != "female"
			)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Invalid Gender";
				return genericResponse;
			}

			// Validate Date of Birth (Age between 6 and 9 years)
			var currentDate = DateTime.Now;
			var age = currentDate.Year - createChildDto.DateOfBirth.Year;

			// If the current date is before the child's birthday this year, subtract 1 from the age
			if (
				currentDate.Month < createChildDto.DateOfBirth.Month
				|| (
					currentDate.Month == createChildDto.DateOfBirth.Month
					&& currentDate.Day < createChildDto.DateOfBirth.Day
				)
			)
			{
				age--;
			}

			if (age < 6 || age > 9)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Child's age must be between 6 and 9 years.";
				return genericResponse;
			}

			var sportHistory = await _unitOfWork
				.Repository<Sport, int>()
				.GetAsync(createChildDto.SportHistoryId);
			if (sportHistory is null)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Invalid sport History";

				return genericResponse;
			}

			var parentSportHistory = await _unitOfWork
				.Repository<Sport, int>()
				.GetAsync(createChildDto.ParentSportHistoryId);
			if (parentSportHistory is null)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Invalid parent sport History";

				return genericResponse;
			}

			var sportPerference = await _unitOfWork
				.Repository<Sport, int>()
				.GetAsync(createChildDto.SportPreferenceId);
			if (sportPerference is null)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Invalid sport Preference";

				return genericResponse;
			}

			//Upload Photo

			createChildDto.FrontImageName = await DocumentSettings.UploadFile(
				createChildDto.FrontImage,
				"Images"
			);
			createChildDto.SideImageName = await DocumentSettings.UploadFile(
				createChildDto.SideImage,
				"Images"
			);

			foreach (var testchild in createChildDto.CreateChildTestDtos)
			{
				var test = await _unitOfWork.Repository<Test, int>().GetAsync(testchild.TestId);
				if (test is null)
				{
					genericResponse.StatusCode = StatusCodes.Status400BadRequest;
					genericResponse.Message = "Invalid Test You enter";

					return genericResponse;
				}
			}

			var mappedChild = _mapper.Map<Child>(createChildDto);
			mappedChild.ImageFrontURL = createChildDto.FrontImageName;
			mappedChild.ImageSideURL = createChildDto.SideImageName;
			mappedChild.AthliQUserId = userId;
			await _unitOfWork.Repository<Child, int>().AddAsync(mappedChild);
			var result = await _unitOfWork.CompleteAsync();

			if (result > 0)
			{
				foreach (var testChild in createChildDto.CreateChildTestDtos)
				{
					var ChildTest = new ChildTest()
					{
						TestId = testChild.TestId,
						ChildId = mappedChild.Id,
						TestResult = testChild.TestResult,
					};

					await _unitOfWork.Repository<ChildTest, int>().AddAsync(ChildTest);
				}
				var resultCreationOfChildTest = await _unitOfWork.CompleteAsync();
				if (resultCreationOfChildTest > 0)
				{
					genericResponse.StatusCode = StatusCodes.Status201Created;
					genericResponse.Message = "Child and its test Results Created Succesfully";
					genericResponse.Data = true;

					return genericResponse;
				}
			}

			genericResponse.StatusCode = StatusCodes.Status200OK;
			genericResponse.Message = "Failed to Create Child and its test Results";
			genericResponse.Data = false;
			return genericResponse;
		}

		public async Task<GenericResponse<bool>> DeleteChildAsync(int childId, string userId)
		{
			var genericResponse = new GenericResponse<bool>();
			var child = await _unitOfWork
				.Repository<Child, int>()
				.Get(c => c.Id == childId && c.AthliQUserId == userId)
				.Result.FirstOrDefaultAsync();
			if (child is null)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Invalid Child Id to delete";

				genericResponse.Data = false;

				return genericResponse;
			}

			child.IsDeleted = true;
			_unitOfWork.Repository<Child, int>().Update(child);
			var result = await _unitOfWork.CompleteAsync();

			if (result > 0)
			{
				genericResponse.StatusCode = StatusCodes.Status200OK;
				genericResponse.Message = "Child Deleted Succesfully";
				genericResponse.Data = true;

				return genericResponse;
			}

			genericResponse.StatusCode = StatusCodes.Status200OK;
			genericResponse.Message = "Failed to Delete Child";
			genericResponse.Data = false;

			return genericResponse;
		}

		public async Task<GenericResponse<ReturnedEvaluateChildDto>> EvaluateDataAsync(int childId)
		{
			var genericResponse = new GenericResponse<ReturnedEvaluateChildDto>();
			var child = await _unitOfWork
				.Repository<Child, int>()
				.Get(c => c.Id == childId && c.IsDeleted != true)
				.Result.Include(c => c.ChildTests)
				.FirstOrDefaultAsync();

			if (child is null)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Cannot Evalute Child Data because it is not exist";

				return genericResponse;
			}

			var preferedSports = await _unitOfWork
				.Repository<Sport, int>()
				.Get(s => s.Id == child.SportPreferenceId)
				.Result.Include(s => s.Category)
				.FirstOrDefaultAsync();

			var listOfPerferedCategory = new List<string>() { preferedSports.Category.Name };

			var parentSportsHistory = await _unitOfWork
				.Repository<Sport, int>()
				.Get(s => s.Id == child.ParentSportHistoryId)
				.Result.Include(s => s.Category)
				.FirstOrDefaultAsync();

			var listOfParentCategory = new List<string> { parentSportsHistory.Category.Name };

			var listOfScores = child
				.ChildTests.OrderBy(ct => ct.TestId)
				.Select(ct => ct.TestResult)
				.ToList();

			var ChildTosendDto = new ChildToSendDto()
			{
				Name = child.Name,
				Gender = child.Gender,
				BirthDate = child.DateOfBirth,
				Height = child.Hieght,
				Weight = child.Weight,
				HasDoctorApproval = child.IsAgreeDoctorApproval,
				HasNormalBloodTest = child.IsNormalBloodTest,
				SchoolType = child.SchoolName,
				PreferredSports = listOfPerferedCategory,
				ParentSportsHistory = listOfParentCategory,
				TestScores = listOfScores,
			};

			var ChildResult = await SendPlayerDataAsync(ChildTosendDto);
			if (ChildResult != null)
			{
				var jsonPolicy = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				};
				var integrationResult = JsonSerializer.Deserialize<AllDataChildJavaDto>(
					ChildResult,
					jsonPolicy
				);

				var result = integrationResult
					.CategoryScores.Select(kvp => new ChildResultIntegratedDto
					{
						Category = kvp.Key,
						Score = kvp.Value,
					})
					.ToList();

				var ResultCategoryOfTheChild = result.OrderByDescending(c => c.Score).ElementAt(0);
				var childResultCategory = new ChildResult()
				{
					ChildId = child.Id,
					CategoryId = await _unitOfWork
						.Repository<Category, int>()
						.Get(c => c.Name == ResultCategoryOfTheChild.Category)
						.Result.Select(c => c.Id)
						.FirstOrDefaultAsync(),
					ResultDate = DateTime.Now,
				};

				await _unitOfWork.Repository<ChildResult, int>().AddAsync(childResultCategory);
				var resultOfCreationChildResult = await _unitOfWork.CompleteAsync();
				if (resultOfCreationChildResult > 0)
				{
					var sports = await _unitOfWork
						.Repository<Sport, int>()
						.Get(s => s.CategoryId == childResultCategory.CategoryId)
						.Result.ToListAsync();
					var returnedEvaluatedData = new ReturnedEvaluateChildDto
					{
						ChildResultIntegratedDto = result,
						FinalResult =
							$"{child.Name}'s Best Category is {integrationResult.BestCategory}",
						MatchedSports = _mapper.Map<List<ResultedSportDto>>(sports),
					};
					genericResponse.StatusCode = StatusCodes.Status200OK;
					genericResponse.Message = "Retreived Result succesfully";
					genericResponse.Data = returnedEvaluatedData;
					return genericResponse;
				}

				genericResponse.StatusCode = StatusCodes.Status200OK;
				genericResponse.Message = "Failed to Add Result Of Child";

				return genericResponse;
			}

			genericResponse.StatusCode = StatusCodes.Status200OK;
			genericResponse.Message = "Failed to retreive Result Of Child";

			return genericResponse;
		}

		#region Test
		//public async Task<GenericResponse<List<ChildResultIntegratedDto>>> EvaluateDataTestAsync(
		//    ChildToSendDto player
		//)
		//{
		//    var genericResponse = new GenericResponse<List<ChildResultIntegratedDto>>();

		//    var ChildResult = await SendPlayerDataAsync(player);

		//    genericResponse.StatusCode = StatusCodes.Status200OK;
		//    genericResponse.Message = "Retreived Result";
		//    return genericResponse;
		//}
		#endregion


		#region ViewAll Without TotalCount
		//public async Task<GenericResponse<List<GetAllChildDto>>> ViewAllChildrenAsync(
		//    string userId,
		//    string? search,
		//    int? pageSize = 5,
		//    int? pageIndex = 1
		//)
		//{
		//    var genericResponse = new GenericResponse<List<GetAllChildDto>>();

		//    if (search != null)
		//    {
		//        var SearchedChildren = await _unitOfWork
		//            .Repository<Child, int>()
		//            .Get(c =>
		//                c.AthliQUserId == userId
		//                && c.Name.ToLower().Contains(search.ToLower())
		//                && c.IsDeleted != true
		//            )
		//            .Result.ToListAsync();

		//        if (SearchedChildren.Count == 0)
		//        {
		//            genericResponse.StatusCode = StatusCodes.Status400BadRequest;
		//            genericResponse.Message = "No Children with this name";

		//            return genericResponse;
		//        }

		//        var mappedFilteredChildren = _mapper.Map<List<GetAllChildDto>>(SearchedChildren);
		//        foreach (var child in mappedFilteredChildren)
		//        {
		//            var childCategory = await _unitOfWork
		//                .Repository<ChildResult, int>()
		//                .Get(cr => cr.ChildId == child.Id)
		//                .Result.FirstOrDefaultAsync();
		//            if (childCategory is null)
		//            {
		//                child.Category = null;
		//            }
		//            else
		//            {
		//                var category = await _unitOfWork
		//                    .Repository<Category, int>()
		//                    .GetAsync(childCategory.CategoryId);
		//                if (category is null)
		//                {
		//                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
		//                    genericResponse.Message = "Invalid Category As Result";
		//                    return genericResponse;
		//                }
		//                child.Category = category.Name;
		//            }
		//        }
		//        genericResponse.StatusCode = StatusCodes.Status200OK;
		//        genericResponse.Message = "Successfully To Search on Children";
		//        genericResponse.Data = mappedFilteredChildren;

		//        return genericResponse;
		//    }

		//    var allChildrenOfUser = await _unitOfWork
		//        .Repository<Child, int>()
		//        .Get(c => c.AthliQUserId == userId && c.IsDeleted != true)
		//        .Result.Skip((pageIndex.Value - 1) * pageSize.Value)
		//        .Take(pageSize.Value)
		//        .OrderByDescending(c => c.CreatedAt)
		//        .ToListAsync();
		//    if (allChildrenOfUser.Count == 0)
		//    {
		//        genericResponse.StatusCode = StatusCodes.Status200OK;
		//        genericResponse.Message = "There are No Children to show";

		//        return genericResponse;
		//    }

		//    var mappedChildren = _mapper.Map<List<GetAllChildDto>>(allChildrenOfUser);

		//    foreach (var child in mappedChildren)
		//    {
		//        var childCategory = await _unitOfWork
		//            .Repository<ChildResult, int>()
		//            .Get(cr => cr.ChildId == child.Id)
		//            .Result.FirstOrDefaultAsync();
		//        if (childCategory is null)
		//        {
		//            child.Category = null;
		//        }
		//        else
		//        {
		//            var category = await _unitOfWork
		//                .Repository<Category, int>()
		//                .GetAsync(childCategory.CategoryId);
		//            if (category is null)
		//            {
		//                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
		//                genericResponse.Message = "Invalid Category As Result";
		//                return genericResponse;
		//            }
		//            child.Category = category.Name;
		//        }
		//    }

		//    genericResponse.StatusCode = StatusCodes.Status200OK;
		//    genericResponse.Message = "Success to retreive all children";
		//    genericResponse.Data = mappedChildren;
		//    return genericResponse;
		//} 
		#endregion

		public async Task<GenericResponse<(int totalCount, List<GetAllChildDto> getAllChildDtos)>> ViewAllChildrenAsync(
			string userId,
			string? search,
			int? pageSize = 5,
			int? pageIndex = 1)
		{
			var genericResponse = new GenericResponse<(int totalCount, List<GetAllChildDto>)>();

			var totalCount = 0;

			if (search != null)
			{
				var SearchedChildren = await _unitOfWork
				.Repository<Child, int>()
				.Get(c =>
					      c.AthliQUserId == userId
					      && c.Name.ToLower().Contains(search.ToLower())
					      && c.IsDeleted != true)
				.Result.ToListAsync();

				if (SearchedChildren.Count == 0)
				{
					genericResponse.StatusCode = StatusCodes.Status400BadRequest;
					genericResponse.Message = "No Children with this name";

					return genericResponse;
				}

				totalCount = SearchedChildren.Count;

				var mappedFilteredChildren = _mapper.Map<List<GetAllChildDto>>(SearchedChildren);
				foreach (var child in mappedFilteredChildren)
				{
					var childCategory = await _unitOfWork.Repository<ChildResult, int>()
						                                 .Get(cr => cr.ChildId == child.Id)
						                                 .Result.FirstOrDefaultAsync();
					if (childCategory is null)
					{
						child.Category = null;
					}
					else
					{
						var category = await _unitOfWork
							.Repository<Category, int>()
							.GetAsync(childCategory.CategoryId);
						if (category is null)
						{
							genericResponse.StatusCode = StatusCodes.Status400BadRequest;
							genericResponse.Message = "Invalid Category As Result";
							return genericResponse;
						}
						child.Category = category.Name;
					}
				}
				genericResponse.StatusCode = StatusCodes.Status200OK;
				genericResponse.Message = "Successfully To Search on Children";
				genericResponse.Data = (totalCount, mappedFilteredChildren);

				return genericResponse;
			}

			var allChildrenOfUser = await _unitOfWork
				.Repository<Child, int>()
				.Get(c => c.AthliQUserId == userId && c.IsDeleted != true)
				.Result.Skip((pageIndex.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync();
			if (allChildrenOfUser.Count == 0)
			{
				genericResponse.StatusCode = StatusCodes.Status200OK;
				genericResponse.Message = "There are No Children to show";

				return genericResponse;
			}

			totalCount = allChildrenOfUser.Count;

			var mappedChildren = _mapper.Map<List<GetAllChildDto>>(allChildrenOfUser);

			foreach (var child in mappedChildren)
			{
				var childCategory = await _unitOfWork
					.Repository<ChildResult, int>()
					.Get(cr => cr.ChildId == child.Id)
					.Result.FirstOrDefaultAsync();
				if (childCategory is null)
				{
					child.Category = null;
				}
				else
				{
					var category = await _unitOfWork
						.Repository<Category, int>()
						.GetAsync(childCategory.CategoryId);
					if (category is null)
					{
						genericResponse.StatusCode = StatusCodes.Status400BadRequest;
						genericResponse.Message = "Invalid Category As Result";
						return genericResponse;
					}
					child.Category = category.Name;
				}
			}

			genericResponse.StatusCode = StatusCodes.Status200OK;
			genericResponse.Message = "Success to retreive all children";
			genericResponse.Data = (totalCount , mappedChildren);
			return genericResponse;
		}
		private async Task<string> SendPlayerDataAsync(object player)
		{
			var jsonOptionsPolicy = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};
			var json = JsonSerializer.Serialize(player, jsonOptionsPolicy);
			var content = new StringContent(json, Encoding.UTF8, "application/json");

			try
			{
				var response = await _httpClient.PostAsync(
					"http://localhost:9091/player/categorize",
					content
				);
				response.EnsureSuccessStatusCode();

				return await response.Content.ReadAsStringAsync();
			}
			catch (Exception ex)
			{
				return $" Error sending data to Drools API: {ex.Message}";
			}
		}
	}
}
