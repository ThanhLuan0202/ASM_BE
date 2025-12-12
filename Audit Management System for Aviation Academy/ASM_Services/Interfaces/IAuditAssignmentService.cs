using ASM_Repositories.Models.AuditAssignmentDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IAuditAssignmentService
    {
        Task<IEnumerable<ViewAuditAssignment>> GetAllAsync();
        Task<ViewAuditAssignment?> GetByIdAsync(Guid assignmentId);
        Task<IEnumerable<ViewAuditAssignment>> GetByAuditIdAsync(Guid auditId);
        Task<IEnumerable<ViewAuditAssignment>> GetByAuditorIdAsync(Guid auditorId);
        Task<IEnumerable<ViewAuditAssignment>> GetByDeptIdAsync(int deptId);
        Task<ViewAuditAssignment> CreateAsync(CreateAuditAssignment dto, Guid userId);
        Task<IEnumerable<BulkCreateAuditAssignmentResponse>> BulkCreateAsync(BulkCreateAuditAssignmentRequest request, Guid userId);
        Task<ViewAuditAssignment?> UpdateAsync(Guid assignmentId, UpdateAuditAssignment dto, Guid userId);
        Task<bool> DeleteAsync(Guid assignmentId, Guid userId);
    }
}

