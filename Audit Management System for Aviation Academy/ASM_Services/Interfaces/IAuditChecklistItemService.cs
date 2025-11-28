using ASM_Repositories.Models.AuditChecklistItemDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IAuditChecklistItemService
    {
        Task<IEnumerable<ViewAuditChecklistItem>> GetByAuditIdAsync(Guid auditId);
        Task<ViewAuditChecklistItem?> GetByIdAsync(Guid auditItemId);
        Task<ViewAuditChecklistItem> CreateAsync(CreateAuditChecklistItem dto);
        Task<ViewAuditChecklistItem?> UpdateAsync(Guid auditItemId, UpdateAuditChecklistItem dto);
        Task<bool> DeleteAsync(Guid auditItemId);
        Task<IEnumerable<ViewAuditChecklistItem>> GetBySectionAsync(int departmentId);
        Task<IEnumerable<ViewAuditChecklistItem>> GetByUserIdAsync(Guid userId);
        Task<ViewAuditChecklistItem?> SetCompliantAsync(Guid auditItemId);
        Task<ViewAuditChecklistItem?> SetNonCompliantAsync(Guid auditItemId);
        Task<IEnumerable<ViewAuditChecklistItem>> CreateFromTemplateAsync(Guid auditId, int deptId);
    }
}
