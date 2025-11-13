using ASM_Repositories.Entities;
using ASM_Repositories.Models.FindingDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IFindingRepository
    {
        Task<IEnumerable<ViewFinding>> GetAllFindingAsync();
        Task<ViewFinding?> GetFindingByIdAsync(Guid id);
        Task<ViewFinding> CreateFindingAsync(CreateFinding dto, Guid? createdByUserId);
        Task<ViewFinding?> UpdateFindingAsync(Guid id, UpdateFinding dto);
        Task<bool> DeleteFindingAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<List<Finding>> GetFindingsAsync(Guid auditId);
        Task<List<ViewFindingByMonth>> GetFindingsByMonthAsync(Guid auditId);
        Task<List<(string Department, int Count)>> GetDepartmentFindingsInCurrentMonthAsync(Guid auditId);
    }
}
