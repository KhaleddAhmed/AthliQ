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
    }
}
