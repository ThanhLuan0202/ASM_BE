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
        Task<ViewAuditChecklistItem> CreateAsync(CreateAuditChecklistItem dto, Guid userId);
        Task<ViewAuditChecklistItem?> UpdateAsync(Guid auditItemId, UpdateAuditChecklistItem dto, Guid userId);
        Task<bool> DeleteAsync(Guid auditItemId, Guid userId);
        Task<IEnumerable<ViewAuditChecklistItem>> GetBySectionAsync(int departmentId);
        Task<IEnumerable<ViewAuditChecklistItem>> GetByUserIdAsync(Guid userId);
        Task<ViewAuditChecklistItem?> SetCompliantAsync(Guid auditItemId, Guid userId);
        Task<ViewAuditChecklistItem?> SetNonCompliantAsync(Guid auditItemId, Guid userId);
        Task<IEnumerable<ViewAuditChecklistItem>> CreateFromTemplateAsync(Guid auditId, int deptId, Guid userId);
    }
}
