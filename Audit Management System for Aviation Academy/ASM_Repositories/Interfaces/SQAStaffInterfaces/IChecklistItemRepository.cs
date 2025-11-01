using ASM_Repositories.Models.ChecklistItemDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.SQAStaffInterfaces
{
    public interface IChecklistItemRepository
    {
        Task<IEnumerable<ViewChecklistItem>> GetAllChecklistItemAsync();
        Task<IEnumerable<ViewChecklistItem>> GetChecklistItemsByTemplateIdAsync(Guid templateId);
        Task<ViewChecklistItem?> GetChecklistItemByIdAsync(Guid id);
        Task<ViewChecklistItem> CreateChecklistItemAsync(CreateChecklistItem dto);
        Task<ViewChecklistItem?> UpdateChecklistItemAsync(Guid id, UpdateChecklistItem dto);
        Task<bool> DeleteChecklistItemAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
