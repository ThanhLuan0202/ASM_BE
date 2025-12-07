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
        Task<ViewAuditPlanAssignment?> GetByIdAsync(int id);
        Task<ViewAuditPlanAssignment> CreateAsync(CreateAuditPlanAssignment dto);
        Task<ViewAuditPlanAssignment?> UpdateAsync(int id, UpdateAuditPlanAssignment dto);
        Task<bool> DeleteAsync(int id);
    }
}
