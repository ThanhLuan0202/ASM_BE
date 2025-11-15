using ASM_Repositories.Models.AuditChecklistItemDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditChecklistItemRepository
    {
        Task<IEnumerable<ViewAuditChecklistItem>> GetByAuditIdAsync(Guid auditId);
        Task<ViewAuditChecklistItem?> GetByIdAsync(Guid auditItemId);
        Task<ViewAuditChecklistItem> CreateAsync(CreateAuditChecklistItem dto);
        Task<ViewAuditChecklistItem?> UpdateAsync(Guid auditItemId, UpdateAuditChecklistItem dto);
        Task<bool> DeleteAsync(Guid auditItemId);
        Task<IEnumerable<ViewAuditChecklistItem>> GetBySectionAsync(string section);
        Task<IEnumerable<ViewAuditChecklistItem>> GetByUserIdAsync(Guid userId);
    }
}
