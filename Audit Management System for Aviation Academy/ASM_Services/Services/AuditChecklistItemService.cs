using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditChecklistItemDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditChecklistItemService : IAuditChecklistItemService
    {
        private readonly IAuditChecklistItemRepository _repo;

        public AuditChecklistItemService(IAuditChecklistItemRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetByAuditIdAsync(Guid auditId)
        {
            return await _repo.GetByAuditIdAsync(auditId);
        }

        public async Task<ViewAuditChecklistItem?> GetByIdAsync(Guid auditItemId)
        {
            return await _repo.GetByIdAsync(auditItemId);
        }

        public async Task<ViewAuditChecklistItem> CreateAsync(CreateAuditChecklistItem dto)
        {
            return await _repo.CreateAsync(dto);
        }

        public async Task<ViewAuditChecklistItem?> UpdateAsync(Guid auditItemId, UpdateAuditChecklistItem dto)
        {
            return await _repo.UpdateAsync(auditItemId, dto);
        }

        public async Task<bool> DeleteAsync(Guid auditItemId)
        {
            return await _repo.DeleteAsync(auditItemId);
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetBySectionAsync(string section)
        {
            return await _repo.GetBySectionAsync(section);
        }
    }
}
