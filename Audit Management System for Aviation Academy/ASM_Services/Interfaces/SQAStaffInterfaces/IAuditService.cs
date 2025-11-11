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
    }
}
