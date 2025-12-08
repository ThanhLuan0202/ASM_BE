using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditPlanAssignmentRepository
    {
        Task<IEnumerable<ViewAuditPlanAssignment>> GetAllAsync();
        Task<ViewAuditPlanAssignment?> GetByIdAsync(Guid id);
        Task<ViewAuditPlanAssignment> CreateAsync(CreateAuditPlanAssignment dto);
        Task<ViewAuditPlanAssignment?> UpdateAsync(Guid id, UpdateAuditPlanAssignment dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ViewAuditPlanAssignment>> GetAssignmentsByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<int> GetAssignmentCountByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<bool> HasActiveAssignmentByAuditorIdAsync(Guid auditorId);
        Task UpdateStatusToArchivedAsync(Guid auditId);
    }
}
