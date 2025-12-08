using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingSeverityDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class FindingSeverityService : IFindingSeverityService
    {
        private readonly IFindingSeverityRepository _repo;
        private readonly IAuditLogService _logService;

        public FindingSeverityService(IFindingSeverityRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<List<ViewFindingSeverity>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewFindingSeverity?> GetByIdAsync(string severity) => _repo.GetByIdAsync(severity);
        public async Task<ViewFindingSeverity> CreateAsync(CreateFindingSeverity dto, Guid userId)
        {
            var created = await _repo.AddAsync(dto);
            var entityId = Guid.TryParse(created.Severity, out var parsed) ? parsed : Guid.NewGuid();
            await _logService.LogCreateAsync(created, entityId, userId, "FindingSeverity");
            return created;
        }
        public async Task<ViewFindingSeverity> UpdateAsync(string severity, UpdateFindingSeverity dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(severity);
            var updated = await _repo.UpdateAsync(severity, dto);
            if (before != null && updated != null)
            {
                var entityId = Guid.TryParse(severity, out var parsed) ? parsed : Guid.NewGuid();
                await _logService.LogUpdateAsync(before, updated, entityId, userId, "FindingSeverity");
            }
            return updated;
        }
        public async Task<bool> DeleteAsync(string severity, Guid userId)
        {
            var before = await _repo.GetByIdAsync(severity);
            var success = await _repo.DeleteAsync(severity);
            if (success && before != null)
            {
                var entityId = Guid.TryParse(severity, out var parsed) ? parsed : Guid.NewGuid();
                await _logService.LogDeleteAsync(before, entityId, userId, "FindingSeverity");
            }
            return success;
        }
    }
}
