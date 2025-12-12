using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using Microsoft.AspNetCore.Http;
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
        Task<(ViewAuditPlanAssignment Assignment, Notification? Notification)> CreateWithNotificationAsync(CreateAuditPlanAssignment dto, Guid userId, List<IFormFile> files);
        Task<ViewAuditPlanAssignment?> UpdateAsync(Guid id, UpdateAuditPlanAssignment dto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task<IEnumerable<ViewAuditPlanAssignment>> GetAssignmentsByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<ValidateAssignmentResponse> ValidateAssignmentAsync(ValidateAssignmentRequest request);
    }
}
