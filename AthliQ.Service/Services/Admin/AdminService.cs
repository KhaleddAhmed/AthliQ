using AthliQ.Core;
using AthliQ.Core.Entities;
using AthliQ.Core.Responses;
using AthliQ.Core.Service.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Service.Services.Admin
{
	public class AdminService : IAdminService
	{
		private readonly IUnitOfWork _unitOfWork;

		public AdminService(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}

		public async Task<GenericResponse<bool>> AcceptUserAsync(string userId)
		{
			var genericResponse = new GenericResponse<bool>();

			var user = await _unitOfWork.Repository<AthliQUser , string>()
				                        .Get(u=>u.Id == userId).Result
										.FirstOrDefaultAsync();
			if(user is null)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "User Is Not Found";
				return genericResponse;
			}

			user.IsAccepted = true;

			_unitOfWork.Repository<AthliQUser, string>().Update(user);
			var returnedRows = await _unitOfWork.CompleteAsync();

			if(returnedRows > 0)
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

			var user = await _unitOfWork.Repository<AthliQUser , string>()
				                        .Get(u=>u.Id == userId).Result
										.FirstOrDefaultAsync();
			if(user is null)
			{
				genericResponse.StatusCode = StatusCodes.Status400BadRequest;
				genericResponse.Message = "User Is Not Found";
				return genericResponse;
			}

			user.IsDeleted = true;

			_unitOfWork.Repository<AthliQUser , string>().Update(user);
			var returnedRows = await _unitOfWork.CompleteAsync();

			if(returnedRows > 0)
			{
				genericResponse.StatusCode= StatusCodes.Status200OK;
				genericResponse.Message = "User Is Deleted";
				genericResponse.Data = true;

				return genericResponse;
			}

			genericResponse.StatusCode = StatusCodes.Status200OK;
			genericResponse.Message = "Could Not Delete This User";
			return genericResponse;
		}

		public async Task<GenericResponse<bool>> RejectUserAsync(string userId)
		{
			var genericResponse = new GenericResponse<bool>();

			var user = await _unitOfWork.Repository<AthliQUser, string>()
										.Get(u => u.Id == userId).Result
										.FirstOrDefaultAsync();
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
