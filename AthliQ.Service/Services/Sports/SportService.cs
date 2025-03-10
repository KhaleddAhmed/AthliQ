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

        public SportService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<GenericResponse<GetSportDto>> CreateSportAsync(CreateSportDto createSportDto)
        {
            var genericResponse = new GenericResponse<GetSportDto>();

            //1.Check if the passed DTO is Null
            if(createSportDto is null)
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
            if(sport is not null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "This Sport Already Exists";
                return genericResponse;
            }
            //If Null,
            //4.Find the passed Category Id (To Check if it is Found or not)
            var category = await _unitOfWork.Repository<Category, int>()
                                            .GetAsync(createSportDto.CategoryId);

            //7.Check if Null (Not Found)
            if(category is null)
            {
                genericResponse.StatusCode = StatusCodes.Status404NotFound;
                genericResponse.Message = "Invalid Category";
                return genericResponse;
            }

            //If Not Null,
            //8.Map the DTO to Domain Model
            var mappedSport = _mapper.Map<Sport>(createSportDto);

            //9.Add and Save the Sport to DB
            await _unitOfWork.Repository<Sport , int>().AddAsync(mappedSport);
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
    }
}
