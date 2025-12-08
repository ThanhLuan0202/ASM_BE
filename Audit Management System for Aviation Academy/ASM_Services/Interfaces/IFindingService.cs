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
        Task<ViewFinding> CreateFindingAsync(CreateFinding dto, Guid userId);
        Task<ViewFinding?> UpdateFindingAsync(Guid id, UpdateFinding dto, Guid userId);
        Task<bool> DeleteFindingAsync(Guid id, Guid userId);
        Task<List<Finding>> GetFindingsAsync(Guid auditId);
        Task<List<(DateTime Date, int Count)>> GetFindingsByMonthAsync(Guid auditId);
        Task<List<(string Department, int Count)>> GetDepartmentFindingsInAuditAsync(Guid auditId);
        Task<IEnumerable<ViewFinding>> GetFindingsByDepartmentAsync(int departmentId);
        Task<IEnumerable<ViewFinding>> GetFindingsByAuditItemIdAsync(Guid auditItemId);
        Task<ViewFinding?> SetReceivedAsync(Guid findingId, Guid userId);
        Task<IEnumerable<ViewFinding>> GetFindingsByAuditIdAsync(Guid auditId);
        Task<IEnumerable<ViewFinding>> GetFindingsByCreatedByAsync(Guid createdBy);
    }
}
