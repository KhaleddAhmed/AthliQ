using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Child;
using AthliQ.Core.Responses;

namespace AthliQ.Core.Service.Contract
{
    public interface IChildService
    {
        Task<GenericResponse<bool>> CreateChildAsync(string userId, CreateChildDto createChildDto);
        Task<GenericResponse<List<GetAllChildDto>>> ViewAllChildrenAsync(
            string userId,
            string? search,
            int? pageSize = 5,
            int? pageIndex = 1
        );

        Task<GenericResponse<bool>> DeleteChildAsync(int childId, string userId);

        Task<GenericResponse<List<ChildResultIntegratedDto>>> EvaluateDataAsync(int childId);
        //Task<GenericResponse<List<ChildResultIntegratedDto>>> EvaluateDataTestAsync(
        //    ChildToSendDto childToSendDto
        //);
    }
}
