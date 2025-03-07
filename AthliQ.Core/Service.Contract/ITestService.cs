using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Test;
using AthliQ.Core.Responses;

namespace AthliQ.Core.Service.Contract
{
    public interface ITestService
    {
        Task<GenericResponse<GetTestDto>> CreateTestAsync(CreateTestDto createTestDto);

        Task<GenericResponse<GetTestDto>> GetTestAsync(int id);

        Task<GenericResponse<GetTestDto>> UpdateTestAsync(UpdateTestDto updateTestDto);

        Task<GenericResponse<bool>> DeleteTestAsync(int id);

        Task<GenericResponse<List<GetAllTestDto>>> GetAllTestAsync(int? categoryId);
    }
}
