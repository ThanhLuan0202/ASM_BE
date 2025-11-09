using ASM_Repositories.Models.AuditScheduleDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IAuditScheduleService
    {
        Task<IEnumerable<ViewAuditSchedule>> GetAllAsync();
        Task<ViewAuditSchedule?> GetByIdAsync(Guid scheduleId);
        Task<IEnumerable<ViewAuditSchedule>> GetByAuditIdAsync(Guid auditId);
        Task<ViewAuditSchedule> CreateAsync(CreateAuditSchedule dto);
        Task<ViewAuditSchedule?> UpdateAsync(Guid scheduleId, UpdateAuditSchedule dto);
        Task<bool> DeleteAsync(Guid scheduleId);
    }
}

