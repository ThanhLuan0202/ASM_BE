using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IAuditPlanAssignmentService
    {
        Task<IEnumerable<ViewAuditPlanAssignment>> GetAllAsync();
        Task<ViewAuditPlanAssignment?> GetByIdAsync(Guid id);
        Task<Notification?> CreateWithNotificationAsync(CreateAuditPlanAssignment dto);
        Task<ViewAuditPlanAssignment?> UpdateAsync(Guid id, UpdateAuditPlanAssignment dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ViewAuditPlanAssignment>> GetAssignmentsByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<ValidateAssignmentResponse> ValidateAssignmentAsync(ValidateAssignmentRequest request);
    }
}
