using ASM_Repositories.Models.AuditScheduleDTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IAuditScheduleService
    {
        Task<IEnumerable<ViewAuditSchedule>> GetAllAsync();
        Task<ViewAuditSchedule?> GetByIdAsync(Guid scheduleId);
        Task<IEnumerable<ViewAuditSchedule>> GetByAuditIdAsync(Guid auditId);
        Task<ViewAuditSchedule> CreateAsync(CreateAuditSchedule dto, Guid userId);
        Task<ViewAuditSchedule?> UpdateAsync(Guid scheduleId, UpdateAuditSchedule dto, Guid userId);
        Task<bool> DeleteAsync(Guid scheduleId, Guid userId);
        Task<List<Guid>> MarkEvidenceDueOverdueAsync(CancellationToken ct = default);
        Task<List<Guid>> MarkCapaDueOverdueAsync(CancellationToken ct = default);
        Task<List<Guid>> MarkDraftReportDueOverdueAsync(CancellationToken ct = default);
        Task<List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>> GetDraftReportDueTomorrowAssignmentsAsync(CancellationToken ct = default);
        Task<List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>> GetCapaDueTomorrowAssignmentsAsync(CancellationToken ct = default);
        Task<List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>> GetEvidenceDueTomorrowAssignmentsAsync(CancellationToken ct = default);
    }
}

