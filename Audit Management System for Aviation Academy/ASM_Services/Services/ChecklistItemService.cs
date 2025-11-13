using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ChecklistItemDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class ChecklistItemService : IChecklistItemService
    {
        private readonly IChecklistItemRepository _repo;

        public ChecklistItemService(IChecklistItemRepository repo)
        {
            _repo = repo;
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

        public async Task<ViewChecklistItem> CreateChecklistItemAsync(CreateChecklistItem dto)
        {
            return await _repo.CreateChecklistItemAsync(dto);
        }

        public async Task<ViewChecklistItem?> UpdateChecklistItemAsync(Guid id, UpdateChecklistItem dto)
        {
            return await _repo.UpdateChecklistItemAsync(id, dto);
        }

        public async Task<bool> DeleteChecklistItemAsync(Guid id)
        {
            return await _repo.DeleteChecklistItemAsync(id);
        }
    }
}
