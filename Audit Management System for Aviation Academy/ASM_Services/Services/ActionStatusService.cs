using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ActionStatusDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class ActionStatusService : IActionStatusService
    {
        private readonly IActionStatusRepository _repo;
        private readonly IAuditLogService _logService;

        public ActionStatusService(IActionStatusRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewActionStatus>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewActionStatus?> GetByIdAsync(string actionStatus) => _repo.GetByIdAsync(actionStatus);
        public async Task<ViewActionStatus> CreateAsync(CreateActionStatus dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            await _logService.LogCreateAsync(created, Guid.Empty, userId, "ActionStatus");
            return created;
        }
        public async Task<ViewActionStatus?> UpdateAsync(string actionStatus, UpdateActionStatus dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(actionStatus);
            var updated = await _repo.UpdateAsync(actionStatus, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, Guid.Empty, userId, "ActionStatus");
            }
            return updated;
        }
        public async Task<bool> DeleteAsync(string actionStatus, Guid userId)
        {
            var before = await _repo.GetByIdAsync(actionStatus);
            var success = await _repo.DeleteAsync(actionStatus);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, Guid.Empty, userId, "ActionStatus");
            }
            return success;
        }
    }
}
