using AthliQ.Core.DTOs.User;
using AthliQ.Core.Responses;

namespace AthliQ.Core.Service.Contract
{
    public interface IAdminService
    {
        Task<GenericResponse<bool>> DeleteUserAsync(string userId);
        Task<GenericResponse<GetAllUsersToReturnDto>> GetAllUsersAsync(int? pageIndex, int? pageSize);
        Task<GenericResponse<StatsDto>> GetStatsAsync();
    }
}
