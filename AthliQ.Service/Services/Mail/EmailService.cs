using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.Entities.Models;
using AthliQ.Core.Service.Contract;
using AthliQ.Service.Helpers;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AthliQ.Service.Services.Mail
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailSettings> _options;
        private readonly SmtpMailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> options, SmtpMailSettings emailSettings)
        {
            _options = options;
            _emailSettings = emailSettings;
        }

        public void SendEmail(Email email)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Value.Email),
                Subject = email.Subject,
            };
            mail.To.Add(MailboxAddress.Parse(email.To));
            mail.From.Add(new MailboxAddress(_options.Value.DisplayName, _options.Value.Email));
            var builder = new BodyBuilder();
            builder.TextBody = email.Body;
            mail.Body = builder.ToMessageBody();
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(
                _options.Value.Host,
                _options.Value.Port,
                MailKit.Security.SecureSocketOptions.StartTls
            );
            smtp.Authenticate(_options.Value.Email, _options.Value.Password);
            smtp.Send(mail);
            smtp.Disconnect(true);
        }

        public async Task SendReportEmailAsync(
            string toEmail,
            string childName,
            byte[] pdfReport,
            byte[] chartImage
        )
        {
            try
            {
                using var client = new System.Net.Mail.SmtpClient(
                    _emailSettings.SmtpServer,
                    _emailSettings.SmtpPort
                );
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(
                    _emailSettings.Username,
                    _emailSettings.Password
                );
                client.EnableSsl = _emailSettings.EnableSsl;

                var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = $"Physical Evaluation Report for {childName}",
                    Body = CreateEmailBody(childName),
                    IsBodyHtml = true,
                };

                message.To.Add(toEmail);

                // Attach PDF report
                if (pdfReport != null && pdfReport.Length > 0)
                {
                    var pdfAttachment = new Attachment(
                        new MemoryStream(pdfReport),
                        $"{childName}_evaluation_report.pdf",
                        "application/pdf"
                    );
                    message.Attachments.Add(pdfAttachment);
                }

                // Attach chart image
                if (chartImage != null && chartImage.Length > 0)
                {
                    var chartAttachment = new Attachment(
                        new MemoryStream(chartImage),
                        $"{childName}_evaluation_chart.png",
                        "image/png"
                    );
                    message.Attachments.Add(chartAttachment);
                }

                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private string CreateEmailBody(string childName)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2c3e50;'>Physical Evaluation Report</h2>
                    <p>Dear Parent/Guardian,</p>
                    <p>We are pleased to provide you with <strong>{childName}'s</strong> physical evaluation report.</p>
                    <p>The attached documents include:</p>
                    <ul>
                        <li>📄 Detailed PDF report with scores and sport recommendations</li>
                        <li>📊 Visual chart showing the distribution of physical attributes</li>
                    </ul>
                    <p>This evaluation assesses your child's performance in:</p>
                    <ul>
                        <li>Speed and Agility</li>
                        <li>Muscular Strength</li>
                        <li>Muscular Endurance</li>
                        <li>Balance</li>
                    </ul>
                    <p>Based on the results, we have recommended suitable sports that match your child's strengths.</p>
                    <p>If you have any questions about the evaluation results, please don't hesitate to contact us.</p>
                    <br>
                    <p>Best regards,<br>
                    The Evaluation Team</p>
                    <hr style='border: 1px solid #eee;'>
                    <p style='font-size: 12px; color: #666;'>
                        This report was generated on {DateTime.Now:dd/MM/yyyy HH:mm}
                    </p>
                </div>
            </body>
            </html>";
        }
    }
}
