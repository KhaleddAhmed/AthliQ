using Microsoft.AspNetCore.Http;

namespace AthliQ.Core.Repository.Contract
{
    public interface IBodyImageAnalysisService
    {
        Task<string?> AnalyzeBodyImagesAsync(IFormFile frontImage, IFormFile sideImage, CancellationToken cancellationToken = default);
    }
}
