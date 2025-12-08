using ASM_Repositories.Models.ChecklistItemNoFindingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IChecklistItemNoFindingService
    {
        Task<IEnumerable<ViewChecklistItemNoFinding>> GetAllAsync();
        Task<ViewChecklistItemNoFinding?> GetByIdAsync(int id);
        Task<ViewChecklistItemNoFinding> CreateAsync(CreateChecklistItemNoFinding dto, Guid userId);
        Task<ViewChecklistItemNoFinding?> UpdateAsync(int id, UpdateChecklistItemNoFinding dto, Guid userId);
        Task<bool> DeleteAsync(int id, Guid userId);
    }
}
