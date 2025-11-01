using ASM_Repositories.Models.FindingDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IFindingService
    {
        Task<IEnumerable<ViewFinding>> GetAllFindingAsync();
        Task<ViewFinding?> GetFindingByIdAsync(Guid id);
        Task<ViewFinding> CreateFindingAsync(CreateFinding dto);
        Task<ViewFinding?> UpdateFindingAsync(Guid id, UpdateFinding dto);
        Task<bool> DeleteFindingAsync(Guid id);
    }
}
