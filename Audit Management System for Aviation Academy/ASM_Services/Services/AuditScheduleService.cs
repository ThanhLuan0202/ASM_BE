using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditScheduleDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditScheduleService : IAuditScheduleService
    {
        private readonly IAuditScheduleRepository _repo;

        public AuditScheduleService(IAuditScheduleRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditSchedule>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditSchedule?> GetByIdAsync(Guid scheduleId) => _repo.GetByIdAsync(scheduleId);
        public Task<IEnumerable<ViewAuditSchedule>> GetByAuditIdAsync(Guid auditId) => _repo.GetByAuditIdAsync(auditId);
        public Task<ViewAuditSchedule> CreateAsync(CreateAuditSchedule dto) => _repo.CreateAsync(dto);
        public Task<ViewAuditSchedule?> UpdateAsync(Guid scheduleId, UpdateAuditSchedule dto) => _repo.UpdateAsync(scheduleId, dto);
        public Task<bool> DeleteAsync(Guid scheduleId) => _repo.DeleteAsync(scheduleId);
    }
}

