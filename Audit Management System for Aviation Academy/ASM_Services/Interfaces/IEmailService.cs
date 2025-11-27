using ASM_Services.Models.Email;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendForLeadAuditor(string toEmail, string auditTitle, string leadFullName, DateTime? fieldworkStart);
        Task SendRejectForAuditor(string toEmail, string auditorFullName, string auditTitle, string leadFullName, string reason);
        Task SendAuditPlanForwardedToDirectorAsync(string toEmail, string directorFullName, string auditTitle, string forwardedByName, string forwardedByRole, string comment);
    }
}

