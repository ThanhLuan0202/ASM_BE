using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditScheduleDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditScheduleService : IAuditScheduleService
    {
        private readonly IAuditScheduleRepository _repo;
        private readonly IAuditLogService _logService;

        public AuditScheduleService(IAuditScheduleRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewAuditSchedule>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditSchedule?> GetByIdAsync(Guid scheduleId) => _repo.GetByIdAsync(scheduleId);
        public Task<IEnumerable<ViewAuditSchedule>> GetByAuditIdAsync(Guid auditId) => _repo.GetByAuditIdAsync(auditId);
        public async Task<ViewAuditSchedule> CreateAsync(CreateAuditSchedule dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            await _logService.LogCreateAsync(created, created.ScheduleId, userId, "AuditSchedule");
            return created;
        }
        public async Task<ViewAuditSchedule?> UpdateAsync(Guid scheduleId, UpdateAuditSchedule dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(scheduleId);
            var updated = await _repo.UpdateAsync(scheduleId, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, scheduleId, userId, "AuditSchedule");
            }
            return updated;
        }
        public async Task<bool> DeleteAsync(Guid scheduleId, Guid userId)
        {
            var before = await _repo.GetByIdAsync(scheduleId);
            var success = await _repo.DeleteAsync(scheduleId);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, scheduleId, userId, "AuditSchedule");
            }
            return success;
        }

        public Task<List<Guid>> MarkEvidenceDueOverdueAsync(CancellationToken ct = default) => _repo.MarkEvidenceDueOverdueAsync(ct);
        public Task<List<Guid>> MarkCapaDueOverdueAsync(CancellationToken ct = default) => _repo.MarkCapaDueOverdueAsync(ct);
        public Task<List<Guid>> MarkDraftReportDueOverdueAsync(CancellationToken ct = default) => _repo.MarkDraftReportDueOverdueAsync(ct);
        public Task<List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>> GetDraftReportDueTomorrowAssignmentsAsync(CancellationToken ct = default) => _repo.GetDraftReportDueTomorrowAssignmentsAsync(ct);
        public Task<List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>> GetCapaDueTomorrowAssignmentsAsync(CancellationToken ct = default) => _repo.GetCapaDueTomorrowAssignmentsAsync(ct);
        public Task<List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>> GetEvidenceDueTomorrowAssignmentsAsync(CancellationToken ct = default) => _repo.GetEvidenceDueTomorrowAssignmentsAsync(ct);
    }
}

