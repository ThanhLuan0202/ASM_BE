using ASM_Repositories.Models.ChecklistTemplateDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.SQAStaffInterfaces
{
    public interface IChecklistTemplateRepository
    {
        Task<IEnumerable<ViewChecklistTemplate>> GetAllChecklistTemplateAsync();
        Task<ViewChecklistTemplate?> GetChecklistTemplateByIdAsync(Guid id);
        Task<ViewChecklistTemplate> CreateChecklistTemplateAsync(CreateChecklistTemplate dto, Guid? createdByUserId);
        Task<ViewChecklistTemplate?> UpdateChecklistTemplateAsync(Guid id, UpdateChecklistTemplate dto);
        Task<bool> DeleteChecklistTemplateAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
