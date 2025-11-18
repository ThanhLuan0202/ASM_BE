using ASM_Services.Models.Email;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendForLeadAuditor(string toEmail, string auditTitle, string leadFullName, DateTime? fieldworkStart);
    }
}

