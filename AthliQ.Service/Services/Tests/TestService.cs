using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core;
using AthliQ.Core.DTOs.Test;
using AthliQ.Core.Entities.Models;
using AthliQ.Core.Responses;
using AthliQ.Core.Service.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AthliQ.Service.Services.Tests
{
    public class TestService : ITestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GenericResponse<GetTestDto>> CreateTestAsync(CreateTestDto createTestDto)
        {
            var genericResponse = new GenericResponse<GetTestDto>();
            if (createTestDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Enter Valid Data";

                return genericResponse;
            }

            var test = await _unitOfWork
                .Repository<Test, int>()
                .Get(t => t.Name == createTestDto.Name || t.ArabicName == createTestDto.ArabicName)
                .Result.FirstOrDefaultAsync();

            var category = await _unitOfWork
                .Repository<Category, int>()
                .GetAsync(createTestDto.CategoryId);
            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Category to Create Test";

                return genericResponse;
            }

            if (test is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "This already Existed Test";

                return genericResponse;
            }

            var mappedTest = _mapper.Map<Test>(createTestDto);
            await _unitOfWork.Repository<Test, int>().AddAsync(mappedTest);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                var returnedTest = _mapper.Map<GetTestDto>(mappedTest);
                returnedTest.CategoryName = category.Name;
                genericResponse.StatusCode = StatusCodes.Status201Created;
                genericResponse.Message = "Test is Created Succesfully";
                genericResponse.Data = returnedTest;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to Create test";

            return genericResponse;
        }

        public async Task<GenericResponse<bool>> DeleteTestAsync(int id)
        {
            var genericResponse = new GenericResponse<bool>();
            var test = await _unitOfWork.Repository<Test, int>().GetAsync(id);
            if (test is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Test Id To Delete";

                return genericResponse;
            }

            _unitOfWork.Repository<Test, int>().Delete(test);
            var result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Test is deleted Succesfully";
                genericResponse.Data = true;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to delete Test";
            return genericResponse;
        }

        public async Task<GenericResponse<List<GetAllTestDto>>> GetAllTestAsync(int? categoryId)
        {
            var genericResponse = new GenericResponse<List<GetAllTestDto>>();
            if (categoryId is not null)
            {
                var category = await _unitOfWork
                    .Repository<Category, int>()
                    .GetAsync(categoryId.Value);
                if (category is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                    genericResponse.Message = "Invalid Catgeory Id to Filter Test By";
                    return genericResponse;
                }

                var Tests = await _unitOfWork
                    .Repository<Test, int>()
                    .Get(t => t.CategoryId == categoryId.Value)
                    .Result.OrderBy(t => t.CreatedAt)
                    .ToListAsync();

                var mappedTests = _mapper.Map<List<GetAllTestDto>>(Tests);

                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "All Filtered Test retreived Succesfully";

                genericResponse.Data = mappedTests;

                return genericResponse;
            }

            var tests = await _unitOfWork
                .Repository<Test, int>()
                .GetAllAsyncAsQueryable()
                .Result.ToListAsync();

            if (tests.Any())
            {
                var mappedTests = _mapper.Map<List<GetAllTestDto>>(tests);
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "All tests retrieved Succesfully";

                genericResponse.Data = mappedTests;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "No tests to retrieve";

            return genericResponse;
        }

        public async Task<GenericResponse<GetTestDto>> GetTestAsync(int id)
        {
            var genericResponse = new GenericResponse<GetTestDto>();
            var test = await _unitOfWork
                .Repository<Test, int>()
                .Get(t => t.Id == id)
                .Result.Include(t => t.Category)
                .FirstOrDefaultAsync();
            if (test is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Test is not found";

                return genericResponse;
            }

            var mappedTest = _mapper.Map<GetTestDto>(test);
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Test Retreived Succesfully";
            genericResponse.Data = mappedTest;

            return genericResponse;
        }

        public async Task<GenericResponse<GetTestDto>> UpdateTestAsync(UpdateTestDto updateTestDto)
        {
            var genericResponse = new GenericResponse<GetTestDto>();
            if (updateTestDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Enter Valid Data Please";
                return genericResponse;
            }

            var test = await _unitOfWork
                .Repository<Test, int>()
                .Get(t => t.Id == updateTestDto.Id)
                .Result.Include(t => t.Category)
                .FirstOrDefaultAsync();
            if (test is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Test Id To Update";
                return genericResponse;
            }

            var category = await _unitOfWork
                .Repository<Category, int>()
                .GetAsync(updateTestDto.CategoryId);
            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Invalid Category to Create Test";

                return genericResponse;
            }

            _mapper.Map(updateTestDto, test);

            test.UpdatedAt = DateTime.Now;
            _unitOfWork.Repository<Test, int>().Update(test);
            var result = await _unitOfWork.CompleteAsync();
            if (result > 0)
            {
                var mappedTest = _mapper.Map<GetTestDto>(test);
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "Test Updated Succesfully";
                genericResponse.Data = mappedTest;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to Update Test";
            return genericResponse;
        }
    }
}
