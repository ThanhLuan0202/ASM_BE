using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditStatusDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditStatusService : IAuditStatusService
    {
        private readonly IAuditStatusRepository _repo;
        private readonly IAuditLogService _logService;

        public AuditStatusService(IAuditStatusRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewAuditStatus>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditStatus?> GetByIdAsync(string auditStatus) => _repo.GetByIdAsync(auditStatus);
        public async Task<ViewAuditStatus> CreateAsync(CreateAuditStatus dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            var entityId = Guid.TryParse(created.AuditStatus1, out var parsed) ? parsed : Guid.NewGuid();
            await _logService.LogCreateAsync(created, entityId, userId, "AuditStatus");
            return created;
        }
        public async Task<ViewAuditStatus?> UpdateAsync(string auditStatus, UpdateAuditStatus dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(auditStatus);
            var updated = await _repo.UpdateAsync(auditStatus, dto);
            if (before != null && updated != null)
            {
                var entityId = Guid.TryParse(auditStatus, out var parsed) ? parsed : Guid.NewGuid();
                await _logService.LogUpdateAsync(before, updated, entityId, userId, "AuditStatus");
            }
            return updated;
        }
        public async Task<bool> DeleteAsync(string auditStatus, Guid userId)
        {
            var before = await _repo.GetByIdAsync(auditStatus);
            var success = await _repo.DeleteAsync(auditStatus);
            if (success && before != null)
            {
                var entityId = Guid.TryParse(auditStatus, out var parsed) ? parsed : Guid.NewGuid();
                await _logService.LogDeleteAsync(before, entityId, userId, "AuditStatus");
            }
            return success;
        }
    }
}

