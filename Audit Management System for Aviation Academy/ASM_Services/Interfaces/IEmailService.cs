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
        Task SendAuditPlanApprovedForCreatorAsync(string toEmail, string creatorFullName, string auditTitle, string directorFullName, DateTime? fieldworkStart, string comment);
        Task SendAuditPlanApprovedForLeadAsync(string toEmail, string leadFullName, string auditTitle, string directorFullName, DateTime? fieldworkStart, string comment);
        Task SendAuditPlanApprovedForAuditorAsync(string toEmail, string auditorFullName, string auditTitle, string leadFullName, string directorFullName, DateTime? fieldworkStart, string comment);
        Task SendAuditPlanApprovedForDepartmentHeadAsync(string toEmail, string deptHeadFullName, string deptName, string auditTitle, DateTime? fieldworkStart, string comment);
        Task SendAuditPlanRejectedForCreatorAsync(string toEmail, string creatorFullName, string auditTitle, string directorFullName, string comment);
    }
}

