using AthliQ.Core;
using AthliQ.Core.DTOs.Sport;
using AthliQ.Core.Entities.Models;
using AthliQ.Core.Responses;
using AthliQ.Core.Service.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Service.Services.Sports
{
    public class SportService : ISportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<GenericResponse<GetSportDto>> CreateSportAsync(CreateSportDto createSportDto)
        {
            var genericResponse = new GenericResponse<GetSportDto>();

            //1.Check if the passed DTO is Null
            if (createSportDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Please Enter A Valid Inputs";
                return genericResponse;
            }

            //If Not Null,
            //2.Find the passed Sport Name (To Check if it's already exists)
            var sport = await _unitOfWork.Repository<Sport, int>()
                                         .Get(s => s.Name == createSportDto.Name || s.ArabicName == createSportDto.ArabicName).Result
                                         .FirstOrDefaultAsync();

            //3.Check if Not Null (Exists)
            if (sport is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "This Sport Already Exists";
                return genericResponse;
            }

            //If Null,
            //4.Find the passed Category Id (To Check if it is Found or not)
            var category = await _unitOfWork.Repository<Category, int>().GetAsync(createSportDto.CategoryId);

            //7.Check if Null (Not Found)
            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Invalid Category";
                return genericResponse;
            }

            //If Not Null(Found),
            //8.Map the DTO to Domain Model
            var mappedSport = _mapper.Map<Sport>(createSportDto);

            //9.Add and Save the Sport to DB
            await _unitOfWork.Repository<Sport, int>().AddAsync(mappedSport);
            var rowNumber = await _unitOfWork.CompleteAsync();

            //10.Check if the returned row number is more than 0 (Successfully Added to DB)
            if (rowNumber > 0)
            {
                //11.Map the Domain Model to DTO
                var returnedSport = _mapper.Map<GetSportDto>(mappedSport);
                returnedSport.CategoryName = category.Name;

                genericResponse.StatusCode = StatusCodes.Status201Created;
                genericResponse.Message = "Sport Created Successfully";

                //12.Return the Response (pass the mapped sport to the response)
                genericResponse.Data = returnedSport;
                return genericResponse;
            }

            //13.If the returned row number is 0 (Failed to create This Sport)
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed To Create This Sport";
            return genericResponse;
        }

        public async Task<GenericResponse<bool>> DeleteSportAsync(int id)
        {
            var genericResponse = new GenericResponse<bool>();

            //1.Find the Sport in DB
            var sport = await _unitOfWork.Repository<Sport, int>().GetAsync(id);

            //2.Check if Null(Not Found)
            if (sport is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "The Sport Is Not Found";
                return genericResponse;
            }

            //If Not Null,
            //3.Remove and Save it in DB
            _unitOfWork.Repository<Sport, int>().Delete(sport);
            var rowNumber = await _unitOfWork.CompleteAsync();

            //4.If the returned row number is more than 0
            if (rowNumber > 0)
            {
                //5.Return the Response
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "The Sport Has Successfully Been Deleted";
                genericResponse.Data = true;
                return genericResponse;
            }

            //6.If less than 0
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed to Delete This Sport";
            return genericResponse;
        }

        public async Task<GenericResponse<List<GetAllSportsDto>>> GetAllSportsAsync(int? categoryId)
        {
            var genericResponse = new GenericResponse<List<GetAllSportsDto>>();

            //1.Check if the Category Id is Not Null
            if (categoryId.HasValue)
            {
                //2.Find the category in DB
                var category = await _unitOfWork.Repository<Category, int>().GetAsync(categoryId.Value);

                //3.Check if Null (Not Found)
                if (category is null)
                {
                    genericResponse.StatusCode = StatusCodes.Status404NotFound;
                    genericResponse.Message = "Invalid Category";
                    return genericResponse;
                }

                //If Not Null (Found),
                //4.Find All the Sports of this Category in DB
                var sports = await _unitOfWork.Repository<Sport, int>()
                                              .Get(s => s.CategoryId == categoryId.Value).Result
                                              .ToListAsync();
                //5.Check if Not Null(Found)
                if (sports?.Count > 0)
                {
                    //6.Map the Domain Model to DTO
                    var mappedSports = _mapper.Map<List<GetAllSportsDto>>(sports);

                    //7.Return the Response 
                    genericResponse.StatusCode = StatusCodes.Status200OK;
                    genericResponse.Message = "All Filtered Sports Have Successfully Been Retrieved";
                    genericResponse.Data = mappedSports;

                    return genericResponse;
                }

                //8.If Null(Not Found)
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "No Sports Found In This Category";
                return genericResponse;
            }

            //If the Category Id is Null
            //9.Find All the Sports in DB
            var allsports = await _unitOfWork.Repository<Sport, int>().GetAllAsync();

            //10.Check if Not Null (Found)
            if (allsports?.Count() > 0)
            {
                //11.Map the Domain Model to DTO
                var returnedSports = _mapper.Map<List<GetAllSportsDto>>(allsports);

                //12.Return the Response
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "All Filtered Sports Have Successfully Been Retrieved";
                genericResponse.Data = returnedSports;
                return genericResponse;
            }
            
            //13.If Null (Not Found)
            genericResponse.StatusCode = StatusCodes.Status404NotFound;
            genericResponse.Message = "No Sports Found";
            return genericResponse;
        }

        public async Task<GenericResponse<GetSportDto>> GetSportAsync(int id)
        {
            var genericResponse = new GenericResponse<GetSportDto>();

            //1.Find the Sport With its Category in DB
            var sport = await _unitOfWork.Repository<Sport, int>()
                                         .Get(s => s.Id == id).Result
                                         .Include(s => s.Category)
                                         .FirstOrDefaultAsync();

            //2.Check If the sport is Null (Not Found)
            if (sport is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "This Sport Is Not Found";
                return genericResponse;
            }

            //If Not Null (Found),
            //3.Map the Domain Model To DTO
            var mappedSport = _mapper.Map<GetSportDto>(sport);

            //4.Return the Response
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "The Sport Has Successfully Been Retrieved";
            genericResponse.Data = mappedSport;

            return genericResponse;
        }

        public async Task<GenericResponse<GetSportDto>> UpdateSportAsync(UpdateSportDto updateSportDto)
        {
            var genericResponse = new GenericResponse<GetSportDto>();

            //1.Check if the passed sport is Null
            if (updateSportDto is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "Please Enter A Valid Inputs";
                return genericResponse;
            }

            //If Not Null,
            //3.Find the sport with category in DB
            var sport = await _unitOfWork.Repository<Sport, int>()
                                         .Get(s => s.Id == updateSportDto.Id).Result
                                         .Include(s => s.Category)
                                         .FirstOrDefaultAsync();

            //4.Check if Null(Not Found)
            if (sport is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "The Category Is Not Found";
                return genericResponse;
            }

            //If Not Null(Found),
            //5.Find the Associated Category
            var category = await _unitOfWork.Repository<Category, int>().GetAsync(updateSportDto.CategoryId);

            //6.Check if Null (Not Found)
            if (category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Invalid Category";
                return genericResponse;
            }

            //If Not Null(Found),
            //7.Map the DTO to the Existing Domain Model(DO NOT create a new instance)
            _mapper.Map(updateSportDto, sport);

            //8.Set the UpdatedAt time
            sport.UpdatedAt = DateTime.Now;

            //9.Update and Save it in DB
            _unitOfWork.Repository<Sport, int>().Update(sport);
            var rowNumber = await _unitOfWork.CompleteAsync();

            //10.Check if the returned row number is more than 0
            if (rowNumber > 0)
            {
                //11.Map the Domain Model to DTO
                var returnedSport = _mapper.Map<GetSportDto>(sport);

                //12.Return the Response
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "The Sport Has Successfully been Modified";
                genericResponse.Data = returnedSport;

                return genericResponse;
            }

            //If less than 0
            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Failed To Modify This Sport";

            return genericResponse;
        }
    }
}
