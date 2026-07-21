using AthliQ.Core.DTOs.Child;
using AthliQ.Core.Service.Contract;
using Microsoft.Extensions.Logging;

namespace AthliQ.Repository.BackgroundJobs
{
    public class ChildReportJob(IReportGenerationService _reportGenerationService, IEmailService _emailService, ILogger<ChildReportJob> _logger)
    {
        //[Queue("reports")]
        //[AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task ExecuteAsync(ReturnedEvaluateChildDto evaluatedData, string childName, string recipientEmail, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Starting report generation for child {ChildName}.", childName);

            cancellationToken.ThrowIfCancellationRequested();

            var pdf = await _reportGenerationService.GeneratePdfReportAsync(evaluatedData, childName);

            cancellationToken.ThrowIfCancellationRequested();

            var chart = await _reportGenerationService.GenerateChartImageAsync(evaluatedData);

            cancellationToken.ThrowIfCancellationRequested();

            await _emailService.SendReportEmailAsync(recipientEmail, childName, pdf, chart);

            _logger.LogInformation("Report for child {ChildName} was successfully sent to {Email}.", childName, recipientEmail);
        }
    }
}
