using ASM_Repositories.Models.ChecklistItemDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IChecklistItemService
    {
        Task<IEnumerable<ViewChecklistItem>> GetAllChecklistItemAsync();
        Task<IEnumerable<ViewChecklistItem>> GetChecklistItemsByTemplateIdAsync(Guid templateId);
        Task<ViewChecklistItem?> GetChecklistItemByIdAsync(Guid id);
        Task<ViewChecklistItem> CreateChecklistItemAsync(CreateChecklistItem dto);
        Task<ViewChecklistItem?> UpdateChecklistItemAsync(Guid id, UpdateChecklistItem dto);
        Task<bool> DeleteChecklistItemAsync(Guid id);
    }
}
