using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core;
using AthliQ.Core.DTOs.User;
using AthliQ.Core.Entities;
using AthliQ.Core.Responses;
using AthliQ.Core.Service.Contract;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AthliQ.Service.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GenericResponse<bool>> AcceptUserAsync(string userId)
        {
            var genericResponse = new GenericResponse<bool>();

            var user = await _unitOfWork
                .Repository<AthliQUser, string>()
                .Get(u => u.Id == userId)
                .Result.FirstOrDefaultAsync();
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "User Is Not Found";
                return genericResponse;
            }

            user.IsAccepted = true;

            _unitOfWork.Repository<AthliQUser, string>().Update(user);
            var returnedRows = await _unitOfWork.CompleteAsync();

            if (returnedRows > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "User Is Accepted";
                genericResponse.Data = true;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Could Not Accept This User";
            return genericResponse;
        }

        public async Task<GenericResponse<bool>> DeleteUserAsync(string userId)
        {
            var genericResponse = new GenericResponse<bool>();

            var user = await _unitOfWork
                .Repository<AthliQUser, string>()
                .Get(u => u.Id == userId)
                .Result.FirstOrDefaultAsync();
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "User Is Not Found";
                return genericResponse;
            }

            user.IsDeleted = true;

            _unitOfWork.Repository<AthliQUser, string>().Update(user);
            var returnedRows = await _unitOfWork.CompleteAsync();

            if (returnedRows > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "User Is Deleted";
                genericResponse.Data = true;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Could Not Delete This User";
            return genericResponse;
        }

        public async Task<GenericResponse<GetAllUsersToReturnDto>> GetAllUsersAsync(
            int? pageIndex,
            int? pageSize
        )
        {
            var genericResponse = new GenericResponse<GetAllUsersToReturnDto>();
            var users = await _unitOfWork
                .Repository<AthliQUser, string>()
                .Get(U => U.IsDeleted != true && U.Email != "Ahmed.Abbas@gmail.com")
                .Result.Skip((pageIndex.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync();
            if (!users.Any())
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "No Users to show";
                return genericResponse;
            }

            var mappedUsers = _mapper.Map<List<GetAllUserDto>>(users);

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Success to retreive all users";
            genericResponse.Data = new GetAllUsersToReturnDto
            {
                GetAllUserDtos = mappedUsers,
                Count = await _unitOfWork
                    .Repository<AthliQUser, string>()
                    .GetAllAsyncAsQueryable()
                    .Result.CountAsync(),
            };

            return genericResponse;
        }

        public async Task<GenericResponse<bool>> RejectUserAsync(string userId)
        {
            var genericResponse = new GenericResponse<bool>();

            var user = await _unitOfWork
                .Repository<AthliQUser, string>()
                .Get(u => u.Id == userId)
                .Result.FirstOrDefaultAsync();
            if (user is null)
            {
                genericResponse.StatusCode = StatusCodes.Status400BadRequest;
                genericResponse.Message = "User Is Not Found";
                return genericResponse;
            }

            user.IsAccepted = false;

            _unitOfWork.Repository<AthliQUser, string>().Update(user);
            var returnedRows = await _unitOfWork.CompleteAsync();

            if (returnedRows > 0)
            {
                genericResponse.StatusCode = StatusCodes.Status200OK;
                genericResponse.Message = "User Is Rejected";
                genericResponse.Data = true;

                return genericResponse;
            }

            genericResponse.StatusCode = StatusCodes.Status200OK;
            genericResponse.Message = "Could Not Reject This User";
            return genericResponse;
        }
    }
}
