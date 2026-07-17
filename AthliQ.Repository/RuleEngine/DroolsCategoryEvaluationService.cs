using AthliQ.Core.DTOs.Child;
using AthliQ.Core.Repository.Contract;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace AthliQ.Repository.RuleEngine
{
    public class DroolsCategoryEvaluationService(HttpClient _httpClient, IOptions<CategoryEvaluationOptions> _options) : ICategoryEvaluationService
    {
        public Task<string?> EvaluateChildAsync(ChildToSendDto child, CancellationToken cancellationToken = default)
        {
            return PostAsync(child, "player/categorize", cancellationToken);
        }

        public Task<string?> EvaluateTestGradesAsync(ChildToSendWithOnlyScoresDto child, CancellationToken cancellationToken = default)
        {
            return PostAsync(child, "player/tests", cancellationToken);
        }

        private async Task<string?> PostAsync<TRequest>(TRequest request, string relativeEndpoint, CancellationToken cancellationToken)
        {
            var baseUrl = _options.Value.BaseUrl;

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Sport evaluation engine URL is not configured.");

            var endpoint = $"{baseUrl.TrimEnd('/')}/{relativeEndpoint.TrimStart('/')}";

            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
    }
}
