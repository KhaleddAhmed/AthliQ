using AthliQ.Core.DTOs.Child;

namespace AthliQ.Core.Repository.Contract
{
    public interface ICategoryEvaluationService
    {
        Task<string?> EvaluateChildAsync(ChildToSendDto child, CancellationToken cancellationToken = default);

        Task<string?> EvaluateTestGradesAsync(ChildToSendWithOnlyScoresDto child, CancellationToken cancellationToken = default);
    }
}
