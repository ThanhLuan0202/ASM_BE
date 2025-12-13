using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.SensitiveAreaLevelDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class SensitiveAreaLevelService : ISensitiveAreaLevelService
    {
        private readonly ISensitiveAreaLevelRepository _repo;
        private readonly IAuditLogService _logService;

        public SensitiveAreaLevelService(ISensitiveAreaLevelRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<List<ViewSensitiveAreaLevel>> GetAllAsync() => _repo.GetAllAsync();
        
        public Task<ViewSensitiveAreaLevel?> GetByIdAsync(string level) => _repo.GetByIdAsync(level);
        
        public async Task<ViewSensitiveAreaLevel> CreateAsync(CreateSensitiveAreaLevel dto, Guid userId)
        {
            var created = await _repo.AddAsync(dto);
            var entityId = Guid.TryParse(created.Level, out var parsed) ? parsed : Guid.NewGuid();
            await _logService.LogCreateAsync(created, entityId, userId, "SensitiveAreaLevel");
            return created;
        }
        
        public async Task<ViewSensitiveAreaLevel> UpdateAsync(string level, UpdateSensitiveAreaLevel dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(level);
            var updated = await _repo.UpdateAsync(level, dto);
            if (before != null && updated != null)
            {
                var entityId = Guid.TryParse(level, out var parsed) ? parsed : Guid.NewGuid();
                await _logService.LogUpdateAsync(before, updated, entityId, userId, "SensitiveAreaLevel");
            }
            return updated;
        }
        
        public async Task<bool> DeleteAsync(string level, Guid userId)
        {
            var before = await _repo.GetByIdAsync(level);
            var success = await _repo.DeleteAsync(level);
            if (success && before != null)
            {
                var entityId = Guid.TryParse(level, out var parsed) ? parsed : Guid.NewGuid();
                await _logService.LogDeleteAsync(before, entityId, userId, "SensitiveAreaLevel");
            }
            return success;
        }
    }
}

