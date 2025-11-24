using ASM_Repositories.Entities;
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
        Task<ViewFinding> CreateFindingAsync(CreateFinding dto, Guid? createdByUserId);
        Task<ViewFinding?> UpdateFindingAsync(Guid id, UpdateFinding dto);
        Task<bool> DeleteFindingAsync(Guid id);
        Task<List<Finding>> GetFindingsAsync(Guid auditId);
        Task<List<(DateTime Date, int Count)>> GetFindingsByMonthAsync(Guid auditId);
        Task<List<(string Department, int Count)>> GetDepartmentFindingsInCurrentMonthAsync(Guid auditId);
        Task<IEnumerable<ViewFinding>> GetFindingsByDepartmentAsync(int departmentId);
        Task<IEnumerable<ViewFinding>> GetFindingsByAuditItemIdAsync(Guid auditItemId);
        Task<ViewFinding?> SetReceivedAsync(Guid findingId);
        Task<IEnumerable<ViewFinding>> GetFindingsByAuditIdAsync(Guid auditId);
    }
}
