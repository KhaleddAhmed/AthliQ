using AthliQ.Core.DTOs.Child;
using AthliQ.Core.Repository.Contract;
using Hangfire;

namespace AthliQ.Repository.BackgroundJobs
{
    public class ChildReportJobScheduler(IBackgroundJobClient _backgroundJobClient) : IChildReportJobScheduler
    {
        public string Enqueue(ReturnedEvaluateChildDto evaluatedData, string childName, string recipientEmail)
        {
            return _backgroundJobClient.Enqueue<ChildReportJob>(job =>
            job.ExecuteAsync(evaluatedData, childName, recipientEmail, CancellationToken.None));
        }
    }
}
