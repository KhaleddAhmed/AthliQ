using AthliQ.Core.Repository.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace AthliQ.Repository.BodyImageAnalysis
{
    public class BodyImageAnalysisService(HttpClient _httpClient, IOptions<BodyImageAnalysisOptions> _options) : IBodyImageAnalysisService
    {
        public async Task<string?> AnalyzeBodyImagesAsync(IFormFile frontImage, IFormFile sideImage, CancellationToken cancellationToken = default)
        {
            using var form = new MultipartFormDataContent();

            using var frontContent = new StreamContent(frontImage.OpenReadStream());

            frontContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(frontImage));

            form.Add(frontContent, "front_image", frontImage.FileName);

            using var sideContent = new StreamContent(sideImage.OpenReadStream());

            sideContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(sideImage));

            form.Add(sideContent, "side_image", sideImage.FileName);

            var baseUrl = _options.Value.BaseUrl;

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Image model URL is not configured.");

            var endpoint = $"{baseUrl.TrimEnd('/')}/analyze";

            using var response = await _httpClient.PostAsync(endpoint, form, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        private static string GetContentType(IFormFile file)
        {
            return string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        }
    }
}
