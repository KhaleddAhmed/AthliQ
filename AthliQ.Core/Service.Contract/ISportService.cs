using AthliQ.Core.DTOs.Sport;
using AthliQ.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthliQ.Core.Service.Contract
{
    public interface ISportService
    {
        Task<GenericResponse<GetSportDto>> CreateSportAsync(CreateSportDto createSportDto);
    }
}
