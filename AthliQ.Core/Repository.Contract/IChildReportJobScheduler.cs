using AthliQ.Core.DTOs.Child;

namespace AthliQ.Core.Repository.Contract
{
    public interface IChildReportJobScheduler
    {
        string Enqueue(ReturnedEvaluateChildDto evaluatedData, string childName, string recipientEmail);
    }
}
