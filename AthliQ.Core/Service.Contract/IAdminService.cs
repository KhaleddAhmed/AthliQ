using AthliQ.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.Service.Contract
{
	public interface IAdminService
	{
		Task<GenericResponse<bool>> AcceptUserAsync(string userId);
		Task<GenericResponse<bool>> RejectUserAsync(string userId);
		Task<GenericResponse<bool>> DeleteUserAsync(string userId);
	}
}
