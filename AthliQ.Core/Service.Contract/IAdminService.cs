using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.User;
using AthliQ.Core.Responses;

namespace AthliQ.Core.Service.Contract
{
    public interface IAdminService
    {
        Task<GenericResponse<bool>> AcceptUserAsync(string userId);
        Task<GenericResponse<bool>> RejectUserAsync(string userId);
        Task<GenericResponse<bool>> DeleteUserAsync(string userId);
        Task<GenericResponse<GetAllUsersToReturnDto>> GetAllUsersAsync(
            int? pageIndex,
            int? pageSize
        );
    }
}
