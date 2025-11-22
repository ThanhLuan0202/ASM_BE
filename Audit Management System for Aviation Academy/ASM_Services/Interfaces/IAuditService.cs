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
        Task<bool> SubmitToLeadAuditorAsync(Guid auditId);
        Task<bool> RejectPlanContentAsync(Guid auditId, Guid approverId, string comment);
        Task<bool> ApproveAndForwardToDirectorAsync(Guid auditId, Guid approverId, string comment);
        Task<bool> ApprovePlanAsync(Guid auditId, Guid approverId, string comment);
        Task<ViewAuditSummary?> GetAuditSummaryAsync(Guid auditId);
        Task<Notification> SubmitAuditAsync(Guid auditId, string pdfUrl, Guid requestedBy);
        Task<Notification> ReportApproveAsync(Guid auditId, Guid userBy, string note);
        Task UpdateReportStatusAndNoteAsync(Guid auditId, string statusAudit, string statusDoc, string note);
        Task<bool> UpdateAuditPlanAsync(Guid auditId, UpdateAuditPlan request);
    }
}
