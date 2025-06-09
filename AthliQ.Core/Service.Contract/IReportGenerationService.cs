using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.DTOs.Child;

namespace AthliQ.Core.Service.Contract
{
    public interface IReportGenerationService
    {
        Task<byte[]> GeneratePdfReportAsync(ReturnedEvaluateChildDto data, string childName);
        Task<byte[]> GenerateChartImageAsync(ReturnedEvaluateChildDto data);
    }
}
