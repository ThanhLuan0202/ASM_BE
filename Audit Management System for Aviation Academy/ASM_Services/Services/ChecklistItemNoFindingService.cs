using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ChecklistItemNoFindingDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class ChecklistItemNoFindingService : IChecklistItemNoFindingService
    {
        private readonly IChecklistItemNoFindingRepository _repo;
        private readonly IAuditLogService _logService;

        public ChecklistItemNoFindingService(IChecklistItemNoFindingRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewChecklistItemNoFinding>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewChecklistItemNoFinding?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public async Task<ViewChecklistItemNoFinding> CreateAsync(CreateChecklistItemNoFinding dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            var entityId = Guid.NewGuid(); // int primary key; use generated Guid for logging context
            await _logService.LogCreateAsync(created, entityId, userId, "ChecklistItemNoFinding");
            return created;
        }
        public async Task<ViewChecklistItemNoFinding?> UpdateAsync(int id, UpdateChecklistItemNoFinding dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateAsync(id, dto);
            if (before != null && updated != null)
            {
                var entityId = Guid.NewGuid();
                await _logService.LogUpdateAsync(before, updated, entityId, userId, "ChecklistItemNoFinding");
            }
            return updated;
        }
        public async Task<bool> DeleteAsync(int id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var success = await _repo.DeleteAsync(id);
            if (success && before != null)
            {
                var entityId = Guid.NewGuid();
                await _logService.LogDeleteAsync(before, entityId, userId, "ChecklistItemNoFinding");
            }
            return success;
        }
    }
}
