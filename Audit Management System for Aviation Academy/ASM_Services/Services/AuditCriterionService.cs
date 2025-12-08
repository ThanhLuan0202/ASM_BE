using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditCriterionDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditCriterionService : IAuditCriterionService
    {
        private readonly IAuditCriterionRepository _repo;
        private readonly IAuditLogService _logService;

        public AuditCriterionService(IAuditCriterionRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewAuditCriterion>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditCriterion?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public async Task<ViewAuditCriterion> CreateAsync(CreateAuditCriterion dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            await _logService.LogCreateAsync(created, created.CriteriaId, userId, "AuditCriterion");
            return created;
        }
        public async Task<ViewAuditCriterion?> UpdateAsync(Guid id, UpdateAuditCriterion dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateAsync(id, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, id, userId, "AuditCriterion");
            }
            return updated;
        }
        public async Task<bool> SoftDeleteAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var success = await _repo.SoftDeleteAsync(id);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, id, userId, "AuditCriterion");
            }
            return success;
        }
    }
}
