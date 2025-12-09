using ASM_Repositories.Models.AuditScheduleDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditScheduleRepository
    {
        Task<IEnumerable<ViewAuditSchedule>> GetAllAsync();
        Task<ViewAuditSchedule?> GetByIdAsync(Guid scheduleId);
        Task<IEnumerable<ViewAuditSchedule>> GetByAuditIdAsync(Guid auditId);
        Task<ViewAuditSchedule> CreateAsync(CreateAuditSchedule dto);
        Task<ViewAuditSchedule?> UpdateAsync(Guid scheduleId, UpdateAuditSchedule dto);
        Task<bool> DeleteAsync(Guid scheduleId);
        Task<bool> ExistsAsync(Guid scheduleId);
        Task UpdateSchedulesAsync(Guid auditId, List<UpdateAuditSchedule>? list);
        Task UpdateStatusToArchivedAsync(Guid auditId);
        Task<int> MarkEvidenceDueOverdueAsync(CancellationToken ct = default);
    }
}

