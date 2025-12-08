using ASM_Repositories.Models.ChecklistTemplateDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IChecklistTemplateService
    {
        Task<IEnumerable<ViewChecklistTemplate>> GetAllChecklistTemplateAsync();
        Task<ViewChecklistTemplate?> GetChecklistTemplateByIdAsync(Guid id);
        Task<ViewChecklistTemplate> CreateChecklistTemplateAsync(CreateChecklistTemplate dto, Guid userId);
        Task<ViewChecklistTemplate?> UpdateChecklistTemplateAsync(Guid id, UpdateChecklistTemplate dto, Guid userId);
        Task<bool> DeleteChecklistTemplateAsync(Guid id, Guid userId);
    }
}
