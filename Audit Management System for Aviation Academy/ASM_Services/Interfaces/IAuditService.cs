using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IAuditService
    {
        Task<IEnumerable<ViewAudit>> GetAllAuditAsync();
        Task<ViewAudit?> GetAuditByIdAsync(Guid id);
        Task<ViewAudit> CreateAuditAsync(CreateAudit dto, Guid? createdByUserId);
        Task<ViewAudit?> UpdateAuditAsync(Guid id, UpdateAudit dto);
        Task<bool> DeleteAuditAsync(Guid id);
        Task<List<ViewAuditPlan>> GetAuditPlansAsync();
        Task<ViewAuditPlan?> GetAuditPlanDetailsAsync(Guid auditId);
        Task<bool> UpdateStatusAsync(Guid auditId, string status);
        Task<Notification> SubmitToLeadAuditorAsync(Guid auditId, Guid userBy);
        Task<List<Notification>> RejectPlanContentAsync(Guid auditId, Guid approverId, string comment);
        Task<Notification> DeclinedPlanContentAsync(Guid auditId, Guid approverId, string comment);
        Task<List<Notification>> ApproveAndForwardToDirectorAsync(Guid auditId, Guid approverId, string comment);
        Task<List<Notification>> ApprovePlanAsync(Guid auditId, Guid approverId, string comment);
        Task<ViewAuditSummary?> GetAuditSummaryAsync(Guid auditId);
        Task<Notification> SubmitAuditAsync(Guid auditId, string pdfUrl, Guid requestedBy);
        Task<Notification> ReportApproveAsync(Guid auditId, Guid userBy, string note);
        Task<Notification> ReportRejectedAsync(Guid auditId, Guid userBy, string note);
        Task<bool> UpdateAuditPlanAsync(Guid auditId, UpdateAuditPlan request);
        Task AuditArchivedAsync(Guid auditId);
        Task<ViewAudit?> UpdateAuditCompleteAsync(Guid auditId, UpdateAuditComplete dto);
        Task<IEnumerable<ViewAudit>> GetAuditsByPeriodAsync(DateTime startDate, DateTime endDate);
    }
}
