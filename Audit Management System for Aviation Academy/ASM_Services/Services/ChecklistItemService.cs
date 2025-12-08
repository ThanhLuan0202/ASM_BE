using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ChecklistItemDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class ChecklistItemService : IChecklistItemService
    {
        private readonly IChecklistItemRepository _repo;
        private readonly IAuditLogService _logService;

        public ChecklistItemService(IChecklistItemRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public async Task<IEnumerable<ViewChecklistItem>> GetAllChecklistItemAsync()
        {
            return await _repo.GetAllChecklistItemAsync();
        }

        public async Task<IEnumerable<ViewChecklistItem>> GetChecklistItemsByTemplateIdAsync(Guid templateId)
        {
            return await _repo.GetChecklistItemsByTemplateIdAsync(templateId);
        }

        public async Task<ViewChecklistItem?> GetChecklistItemByIdAsync(Guid id)
        {
            return await _repo.GetChecklistItemByIdAsync(id);
        }

        public async Task<ViewChecklistItem> CreateChecklistItemAsync(CreateChecklistItem dto, Guid userId)
        {
            var created = await _repo.CreateChecklistItemAsync(dto);
            await _logService.LogCreateAsync(created, created.ItemId, userId, "ChecklistItem");
            return created;
        }

        public async Task<ViewChecklistItem?> UpdateChecklistItemAsync(Guid id, UpdateChecklistItem dto, Guid userId)
        {
            var before = await _repo.GetChecklistItemByIdAsync(id);
            var updated = await _repo.UpdateChecklistItemAsync(id, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, id, userId, "ChecklistItem");
            }
            return updated;
        }

        public async Task<bool> DeleteChecklistItemAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetChecklistItemByIdAsync(id);
            var success = await _repo.DeleteChecklistItemAsync(id);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, id, userId, "ChecklistItem");
            }
            return success;
        }
    }
}
