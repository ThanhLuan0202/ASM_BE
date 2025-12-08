using ASM_Repositories.Models.ChecklistItemNoFindingDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IChecklistItemNoFindingRepository
    {
        Task<IEnumerable<ViewChecklistItemNoFinding>> GetAllAsync();
        Task<ViewChecklistItemNoFinding?> GetByIdAsync(int id);
        Task<ViewChecklistItemNoFinding> CreateAsync(CreateChecklistItemNoFinding dto);
        Task<ViewChecklistItemNoFinding?> UpdateAsync(int id, UpdateChecklistItemNoFinding dto);
        Task<bool> DeleteAsync(int id);
        Task UpdateStatusToArchivedAsync(Guid auditId);
    }
}
