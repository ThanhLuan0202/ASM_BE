using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditChecklistItemDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditChecklistItemService : IAuditChecklistItemService
    {
        private readonly IAuditChecklistItemRepository _repo;
        private readonly IAuditLogService _logService;

        public AuditChecklistItemService(IAuditChecklistItemRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetByAuditIdAsync(Guid auditId)
        {
            return await _repo.GetByAuditIdAsync(auditId);
        }

        public async Task<ViewAuditChecklistItem?> GetByIdAsync(Guid auditItemId)
        {
            return await _repo.GetByIdAsync(auditItemId);
        }

        public async Task<ViewAuditChecklistItem> CreateAsync(CreateAuditChecklistItem dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            await _logService.LogCreateAsync(created, created.AuditItemId, userId, "AuditChecklistItem");
            return created;
        }

        public async Task<ViewAuditChecklistItem?> UpdateAsync(Guid auditItemId, UpdateAuditChecklistItem dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(auditItemId);
            var updated = await _repo.UpdateAsync(auditItemId, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, auditItemId, userId, "AuditChecklistItem");
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(Guid auditItemId, Guid userId)
        {
            var before = await _repo.GetByIdAsync(auditItemId);
            var success = await _repo.DeleteAsync(auditItemId);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, auditItemId, userId, "AuditChecklistItem");
            }
            return success;
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetBySectionAsync(int departmentId)
        {
            return await _repo.GetBySectionAsync(departmentId);
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetByUserIdAsync(Guid userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task<ViewAuditChecklistItem?> SetCompliantAsync(Guid auditItemId, Guid userId)
        {
            var before = await _repo.GetByIdAsync(auditItemId);
            var updated = await _repo.SetCompliantAsync(auditItemId);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, auditItemId, userId, "AuditChecklistItem");
            }
            return updated;
        }

        public async Task<ViewAuditChecklistItem?> SetNonCompliantAsync(Guid auditItemId, Guid userId)
        {
            var before = await _repo.GetByIdAsync(auditItemId);
            var updated = await _repo.SetNonCompliantAsync(auditItemId);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, auditItemId, userId, "AuditChecklistItem");
            }
            return updated;
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> CreateFromTemplateAsync(Guid auditId, int deptId, Guid userId)
        {
            var created = await _repo.CreateFromTemplateAsync(auditId, deptId);
            if (created != null)
            {
                await _logService.LogCreateAsync(created, auditId, userId, "AuditChecklistItem");
            }
            return created;
        }
    }
}
