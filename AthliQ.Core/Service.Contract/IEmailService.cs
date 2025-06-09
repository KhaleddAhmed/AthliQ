using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthliQ.Core.Entities.Models;

namespace AthliQ.Core.Service.Contract
{
    public interface IEmailService
    {
        public void SendEmail(Email email);
        Task SendReportEmailAsync(
            string toEmail,
            string childName,
            byte[] pdfReport,
            byte[] chartImage
        );
    }
}
