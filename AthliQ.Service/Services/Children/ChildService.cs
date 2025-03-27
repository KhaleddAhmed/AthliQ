using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AthliQ.Core;
using AthliQ.Core.DTOs.Child;
using AthliQ.Core.DTOs.Sport;
using AthliQ.Core.DTOs.Test;
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

        public async Task<GenericResponse<CreationOfChildReturnDto>> CreateChildAsync(
            string userId,
            CreateChildDto createChildDto
        )
        {
            var genericResponse = new GenericResponse<CreationOfChildReturnDto>();

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

            if (createChildDto.CreateChildTestDtos[0].TestResult < 65 || createChildDto.CreateChildTestDtos[0].TestResult > 140)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for Standing Long Jump Test";
                return genericResponse;
            }

            if (createChildDto.CreateChildTestDtos[1].TestResult < 0.0 || createChildDto.CreateChildTestDtos[1].TestResult > 10.0)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for Sit-and-Reach Flexibility Test";
                return genericResponse;
            }

            if (createChildDto.CreateChildTestDtos[2].TestResult < 2.49 || createChildDto.CreateChildTestDtos[2].TestResult > 10.0)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for One-Leg Stand Test";
                return genericResponse;
            }

            if (createChildDto.CreateChildTestDtos[3].TestResult < 5.48 || createChildDto.CreateChildTestDtos[3].TestResult > 22.0)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for Sit-up Test";
                return genericResponse;
            }

            if (createChildDto.CreateChildTestDtos[4].TestResult < 125 || createChildDto.CreateChildTestDtos[4].TestResult > 335)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for Medicine Ball Push (1 kg) from Standing Position Test";
                return genericResponse;
            }

            if (createChildDto.CreateChildTestDtos[5].TestResult < 3 || createChildDto.CreateChildTestDtos[5].TestResult > 9)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for Straight-Line Walking (3 meters) Test";
                return genericResponse;
            }

            if (createChildDto.CreateChildTestDtos[6].TestResult < 5.485 || createChildDto.CreateChildTestDtos[6].TestResult > 10.295)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for 30-Meter Sprint Test";
                return genericResponse;
            }

            if (createChildDto.CreateChildTestDtos[7].TestResult < 5.1 || createChildDto.CreateChildTestDtos[7].TestResult > 9.6)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Out Of Range Input for 15-Meter Zigzag Run Test";
                return genericResponse;
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
                    var creationOfChildToReturnDto = new CreationOfChildReturnDto()
                    {
                        ChildId = mappedChild.Id,
                        IsCreated = true,
                    };
                    genericResponse.StatusCode = StatusCodes.Status201Created;
                    genericResponse.Message = "Child and its test Results Created Succesfully";
                    genericResponse.Data = creationOfChildToReturnDto;

                    return genericResponse;
                }
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to Create Child and its test Results";
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
                .Include(c => c.ChildResults)
                .FirstOrDefaultAsync();

            if (child is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Cannot Evalute Child Data because it is not exist";

                return genericResponse;
            }

            if (child.ChildResults.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "This Child is already Evaluated";
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
                Height = child.Height,
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

                if (integrationResult is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "Error in integrating with AI Evaluator";

                    return genericResponse;
                }

                if (integrationResult.Errors is not null)
                {
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = $"{integrationResult.Errors.First()}";

                    return genericResponse;
                }

                var result = integrationResult
                    .CategoryScores.Select(kvp => new ChildResultIntegratedDto
                    {
                        Category = kvp.Key,
                        Score = kvp.Value,
                    })
                    .ToList();

                var resultWithPercentage = integrationResult
                    .CategoryPercentages.Select(rp => new ChildResultWithPercentagesDto
                    {
                        Category = rp.Key,
                        Percentage = rp.Value,
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
                        ChildResultWithPercentagesDtos = resultWithPercentage,
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

        public async Task<GenericResponse<GetAllChildWithTotalCountDto>> ViewAllChildrenAsync(
            string userId,
            string? search,
            int? pageSize = 5,
            int? pageIndex = 1
        )
        {
            var genericResponse = new GenericResponse<GetAllChildWithTotalCountDto>();
            List<GetAllChildDto> getAllChildDtos = new List<GetAllChildDto>();
            var totalCount = await _unitOfWork
                .Repository<Child, int>()
                .Get(c => c.AthliQUserId == userId && c.IsDeleted != true)
                .Result.CountAsync();
            if (search is not null)
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
                        child.Category = null;
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

                        getAllChildDtos.Add(child);
                    }
                }
                var returnedSearchedData = new GetAllChildWithTotalCountDto()
                {
                    TotalCount = totalCount,
                    Children = getAllChildDtos,
                };

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Successfully To Search on Children";
                genericResponse.Data = returnedSearchedData;

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
                    child.Category = null;
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
                    getAllChildDtos.Add(child);
                }
            }

            var returnedData = new GetAllChildWithTotalCountDto()
            {
                TotalCount = totalCount,
                Children = getAllChildDtos,
            };
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success to retreive all children";
            genericResponse.Data = returnedData;
            return genericResponse;
        }

        public async Task<GenericResponse<GetChildDetailsDto>> ViewChildAsync(int childId)
        {
            var genericResponse = new GenericResponse<GetChildDetailsDto>();

            //Find the Child with the passed Id in the DB with his tests and Resulted Category
            var child = await _unitOfWork
                .Repository<Child, int>()
                .Get(c => c.Id == childId && c.IsDeleted == false)
                .Result.Include(c => c.ChildTests)
                .Include(c => c.ChildResults)
                .FirstOrDefaultAsync();

            //Check if the child is Null (Not Found)
            if (child is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Child Is Not Found";
                return genericResponse;
            }

            //If Not Null (Found),

            //Find the Prefered Sports and Parent Sport in DB
            var preferedSport = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.Id == child.SportPreferenceId)
                .Result.FirstOrDefaultAsync();
            if (preferedSport is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Prefered Sport Found";
                return genericResponse;
            }
            var perferedSportsList = new List<string>() { preferedSport.Name };

            var parentSportHistory = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.Id == child.ParentSportHistoryId)
                .Result.FirstOrDefaultAsync();
            if (parentSportHistory is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Sport For The Parent Found";
                return genericResponse;
            }

            var parentSportsHistoryList = new List<string> { parentSportHistory.Name };

            //Find Child Tests in DB
            var childTests = await _unitOfWork
                .Repository<ChildTest, int>()
                .Get(ct => ct.ChildId == child.Id)
                .Result.ToListAsync();

            var testWithValueList = new List<TestWithValueDto>();

            if (childTests?.Count > 0)
            {
                foreach (var childTest in childTests)
                {
                    var test = await _unitOfWork
                        .Repository<Test, int>()
                        .Get(t => t.Id == childTest.TestId)
                        .Result.FirstOrDefaultAsync();
                    if (test is null)
                    {
                        genericResponse.StatusCode = StatusCodes.Status200OK;
                        genericResponse.Message = "No Test Values Found";
                        return genericResponse;
                    }

                    testWithValueList.Add(
                        new TestWithValueDto()
                        {
                            Name = test.Name,
                            TestResult = childTest.TestResult,
                        }
                    );
                }
            }

            //Find Child Result in DB
            var childResult = await _unitOfWork
                .Repository<ChildResult, int>()
                .Get(cr => cr.ChildId == child.Id)
                .Result.FirstOrDefaultAsync();
            if (childResult is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Child Is Not Evaluated To Have A Result";
                return genericResponse;
            }

            var category = await _unitOfWork
                .Repository<Category, int>()
                .Get(c => c.Id == childResult.CategoryId)
                .Result.Select(c => c.Name)
                .FirstOrDefaultAsync();
            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Category Is Found";
                return genericResponse;
            }

            var sports = await _unitOfWork
                .Repository<Sport, int>()
                .Get(s => s.CategoryId == childResult.CategoryId)
                .Result.Select(s => s.Name)
                .ToListAsync();
            if (!sports.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Sport Is Found";
                return genericResponse;
            }

            var returnedChild = new GetChildDetailsDto()
            {
                Name = child.Name,
                Gender = child.Gender,
                DateOfBirth = child.DateOfBirth,
                SchoolName = child.SchoolName,
                ClubName = child.ClubName,
                Height = child.Height,
                Weight = child.Weight,
                PreferredSports = perferedSportsList,
                ParentSportsHistory = parentSportsHistoryList,
                Tests = testWithValueList,
                Category = category,
                Sports = sports,
            };
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Child Is Successfully Retrieved";
            genericResponse.Data = returnedChild;

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
