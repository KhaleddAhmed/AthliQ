using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core;
using AthliQ.Core.DTOs.Category;
using AthliQ.Core.Entities.Models;
using AthliQ.Core.Responses;
using AthliQ.Core.Service.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AthliQ.Service.Services.Categories
{
    //Controller=>Service
    //Service=>UnitOf work
    //Unit of Work=>Create Repos
    //Repo=>DBContext
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GenericResponse<GetCategoryDto>> CreateCategoryAsync(
            CreateCategoryDto createCategoryDto
        )
        {
            //1.check if this model not null
            var genericResponse = new GenericResponse<GetCategoryDto>();
            if (createCategoryDto == null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Please enter valid Data";

                return genericResponse;
            }

            var category = await _unitOfWork
                .Repository<Category, int>()
                .Get(c => c.Name == createCategoryDto.Name)
                .Result.FirstOrDefaultAsync();
            if (category is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Already existed Category";

                return genericResponse;
            }

            var createdCategory = _mapper.Map<CreateCategoryDto, Category>(createCategoryDto);
            await _unitOfWork.Repository<Category, int>().AddAsync(createdCategory);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status201Created;
                genericResponse.Message = "Category Created Succesfully";

                var mappedCategory = _mapper.Map<Category, GetCategoryDto>(createdCategory);

                genericResponse.Data = mappedCategory;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to Create Category";

            return genericResponse;
        }
    }
}
