using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditRepository
    {
        Task<IEnumerable<ViewAudit>> GetAllAuditAsync();
        Task<ViewAudit?> GetAuditByIdAsync(Guid id);
        Task<ViewAudit> CreateAuditAsync(CreateAudit dto, Guid? createdByUserId);
        Task<ViewAudit?> UpdateAuditAsync(Guid id, UpdateAudit dto);
        Task<bool> DeleteAuditAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<ViewAuditPlan>> GetAllAuditPlansAsync();
        Task<ViewAuditPlan?> GetAuditPlanByIdAsync(Guid auditId);
        Task<bool> UpdateStatusAsync(Guid auditId, string status);
        Task<bool> SubmitToLeadAuditorAsync(Guid auditId);
        Task<bool> RejectPlanContentAsync(Guid auditId, Guid approverId, string comment);
        Task<bool> DeclinedPlanContentAsync(Guid auditId, Guid approverId, string comment);
        Task<bool> ApproveAndForwardToDirectorAsync(Guid auditId, Guid approverId, string comment);
        Task<bool> ApprovePlanAsync(Guid auditId, Guid approverId, string comment);
        Task<UserAccount?> GetLeadAuditorAsync(Guid auditId);
        Task SaveChangesAsync();
        Task<Audit?> UpdateStatusByAuditIdAsync(Guid auditId, string status);
        Task UpdateAuditPlanAsync(Audit audit, UpdateAudit? updateAudit);
        Task<Audit?> GetAuditPlanAsync(Guid auditId);
        Task UpdateStatusToArchivedAsync(Guid auditId);
        Task<ViewAudit?> UpdateAuditCompleteAsync(Guid auditId, UpdateAuditComplete dto);
    }
}
