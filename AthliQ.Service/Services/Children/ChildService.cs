﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core;
using AthliQ.Core.DTOs.Child;
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

        public ChildService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<GenericResponse<List<GetAllChildDto>>> ViewAllChildrenAsync(
            string userId,
            string? search,
            int? pageSize = 5,
            int? pageIndex = 1
        )
        {
            var genericResponse = new GenericResponse<List<GetAllChildDto>>();

            if (search != null)
            {
                var SearchedChildren = await _unitOfWork
                    .Repository<Child, int>()
                    .Get(c =>
                        c.AthliQUserId == userId
                        && c.Name.ToLower().Contains(search.ToLower())
                        && c.IsDeleted != true
                    )
                    .Result.ToListAsync();

                if (SearchedChildren.Count == 0)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "No Children with this name";

                    return genericResponse;
                }

                var mappedFilteredChildren = _mapper.Map<List<GetAllChildDto>>(SearchedChildren);
                foreach (var child in mappedFilteredChildren)
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
                genericResponse.Message = "Successfully To Search on Children";
                genericResponse.Data = mappedFilteredChildren;

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
            genericResponse.Data = mappedChildren;
            return genericResponse;
        }
    }
}
