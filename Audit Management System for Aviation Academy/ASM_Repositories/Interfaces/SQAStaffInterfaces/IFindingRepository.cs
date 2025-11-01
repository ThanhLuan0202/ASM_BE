using ASM_Repositories.Models.FindingDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.SQAStaffInterfaces
{
    public interface IFindingRepository
    {
        Task<IEnumerable<ViewFinding>> GetAllFindingAsync();
        Task<ViewFinding?> GetFindingByIdAsync(Guid id);
        Task<ViewFinding> CreateFindingAsync(CreateFinding dto, Guid? createdByUserId);
        Task<ViewFinding?> UpdateFindingAsync(Guid id, UpdateFinding dto);
        Task<bool> DeleteFindingAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
