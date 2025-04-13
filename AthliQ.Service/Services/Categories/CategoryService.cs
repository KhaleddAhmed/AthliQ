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
using static System.Net.Mime.MediaTypeNames;

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

        public async Task<GenericResponse<GetCategoryDto>> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var genericResponse = new GenericResponse<GetCategoryDto>();

            //1.check if the passed category is null
            if (createCategoryDto == null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Please enter valid Data";

                return genericResponse;
            }

            //2.If the Model is not null , get the Category Name
            var category = await _unitOfWork.Repository<Category, int>()
                                            .Get(c => c.Name == createCategoryDto.Name)
                                            .Result.FirstOrDefaultAsync();
            //3.Check if the Category Exists
            if (category is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Already existed Category";

                return genericResponse;
            }

			var categoryAr = await _unitOfWork.Repository<Category, int>()
										   .Get(c => c.ArabicName == createCategoryDto.ArabicName)
										   .Result.FirstOrDefaultAsync();
			
			if (categoryAr is not null)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "Already existed Category";

				return genericResponse;
			}

			//If the Category does Not Exists ,
			//4.Map the DTO to Domain Model
			var createdCategory = _mapper.Map<CreateCategoryDto, Category>(createCategoryDto);

            //5.Add it to DB and SaveChanges
            await _unitOfWork.Repository<Category, int>().AddAsync(createdCategory);
            var result = await _unitOfWork.CompleteAsync();

            //6.If the Returned Row Number is More than 0 (Successfully Added to DB)
            if (result > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status201Created;
                genericResponse.Message = "Category Created Successfully";

                //7.Map the Domain Model to DTO
                var mappedCategory = _mapper.Map<Category, GetCategoryDto>(createdCategory);

                //8.Pass the Mapped Category (DTO) to the Response
                genericResponse.Data = mappedCategory;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to Create Category";

            return genericResponse;
        }

        public async Task<GenericResponse<List<GetAllCategoriesDto>>> GetAllCategoriesAsync()
        {
            var genericResponse = new GenericResponse<List<GetAllCategoriesDto>>();

            //1.Find All Categories in DB
            var categories = await _unitOfWork.Repository<Category, int>().GetAllAsync();

            //2.Check if Null (No Categories Found)
            if (categories is null || categories.Count() <= 0)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Categories Found";
                return genericResponse;
            }

            //If Not Null,
            //3.Map the Domain Model to DTO
            var mappedCategories = _mapper.Map<List<GetAllCategoriesDto>>(categories);

            //4.Return the Response
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "All Categories Have Successfully Been Retrieved";
            genericResponse.Data = mappedCategories;

            return genericResponse;
        }

        public async Task<GenericResponse<GetCategoryWithSportsDto>> GetCategoryWithSportsAsync(int id)
        {
            var genericResponse = new GenericResponse<GetCategoryWithSportsDto>();

            //1.Find the Category with Sports in DB
            var category = await _unitOfWork.Repository<Category, int>()
                                            .Get(c => c.Id == id).Result
                                            .Include(c => c.Sports)
                                            .FirstOrDefaultAsync();

            //2.Check if the Category is Null
            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "The Category Is Not Found";
                return genericResponse;
            }

            ///3.Find All the Associated Sports in DB
            ///var sports = await _unitOfWork.Repository<Sport, int>()
            ///                              .Get(s=>s.CategoryId == id).Result
            ///                              .ToListAsync();
            ///4.Check If the Sports Are Null
            ///if (sports is null || sports.Count <= 0)
            ///{
            ///    genericResponse.StatusCode = StatusCodes.Status404NotFound;
            ///    genericResponse.Message = "No Sports Found In This Category";
            ///    return genericResponse;
            ///}

            //5.If not Null ,
            //6.Map the Domain Model to DTO
            var mappedCategory = _mapper.Map<GetCategoryWithSportsDto>(category);

            //7.Return Response
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "The Category Has Successfully Been Retrieved";
            genericResponse.Data = mappedCategory;

            return genericResponse;
        }

        public async Task<GenericResponse<GetCategoryDto>> GetCategoryAsync(int id)
        {
            var genericResponse = new GenericResponse<GetCategoryDto>();

            //1.Find the Category in DB
            var category = await _unitOfWork.Repository<Category, int>().GetAsync(id);

            //2.Check if the Category is Null
            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "The Category Is Not Found";
                return genericResponse;
            }

            //3.If not Null ,
            //4.Map the Domain Model to DTO
            var mappedCategory = _mapper.Map<GetCategoryDto>(category);

            //5.Return the Response
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "The Category Is Successfully Retrieved";
            genericResponse.Data = mappedCategory;

            return genericResponse;
        }

        public async Task<GenericResponse<GetCategoryDto>> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
        {
            var genericResponse = new GenericResponse<GetCategoryDto>();

            //1.Check if the passed category is Null (empty inputs) 
            if (updateCategoryDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Please Enter A Valid Inputs";
                return genericResponse;
            }

            //If Not Null,
            //2.Find the category in DB
            var category = await _unitOfWork.Repository<Category, int>()
                                            .Get(c => c.Id == updateCategoryDto.Id).Result
                                            .FirstOrDefaultAsync();

            //3.Check if the category is Null(Not Found)
            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "The Category Is Not Found";
                return genericResponse;
            }

            //If Not Null,
            //4.Map DTO to the existing category (DO NOT create a new instance)
            _mapper.Map(updateCategoryDto, category);

            //5.Set the UpdatedAt Time
            category.UpdatedAt = DateTime.Now;

            //6.Update and Save it in DB
            _unitOfWork.Repository<Category, int>().Update(category);
            var rowNumber = await _unitOfWork.CompleteAsync();

            //6.Check if the returned row number is more than 0
            if (rowNumber > 0)
            {
                //Map the Domain Model to DTO
                var returnedCategory = _mapper.Map<GetCategoryDto>(category);

                //Return the Response
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "The Category Has Successfully been Modified";
                genericResponse.Data = returnedCategory;
                return genericResponse;
            }

            //If less than 0
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed To Modify This Category";
            return genericResponse;
        }

        public async Task<GenericResponse<bool>> DeleteCategoryAsync(int id)
        {
            var genericResponse = new GenericResponse<bool>();

            //1.Find the Category in DB
            var category = await _unitOfWork.Repository<Category, int>().GetAsync(id);

            //2.Check if Null(Not Found)
            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "The Category Is Not Found";
                return genericResponse;
            }

            //If Not Null,
            //3.Remove and Save it in DB
            _unitOfWork.Repository<Category, int>().Delete(category);
            var rowNumber = await _unitOfWork.CompleteAsync();

            //4.If the returned row number is more than 0
            if (rowNumber > 0)
            {
                //5.Return the Response
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "The Category Has Successfully Been Deleted";
                genericResponse.Data = true;
                return genericResponse;
            }

            //6.If less than 0
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to Delete This Category";
            return genericResponse;
        }
    }
}
