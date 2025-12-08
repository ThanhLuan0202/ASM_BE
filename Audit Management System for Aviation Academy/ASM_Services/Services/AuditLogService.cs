using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditLogDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repo;
        private readonly IUsersRepository _usersRepo;

        public AuditLogService(IAuditLogRepository repo, IUsersRepository usersRepo)
        {
            _repo = repo;
            _usersRepo = usersRepo;
        }

        public Task<IEnumerable<ViewAuditLog>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditLog?> GetByIdAsync(Guid logId) => _repo.GetByIdAsync(logId);
        public Task<ViewAuditLog> CreateAsync(CreateAuditLog dto) => _repo.CreateAsync(dto);
        public Task<ViewAuditLog?> UpdateAsync(Guid logId, UpdateAuditLog dto) => _repo.UpdateAsync(logId, dto);
        public Task<bool> DeleteAsync(Guid logId) => _repo.DeleteAsync(logId);
        public async Task LogCreateAsync<T>(T newData, Guid entityId, Guid userId, string entityType)
        {
            var user = await _usersRepo.GetUserShortInfoAsync(userId);
            var log = new AuditLog
            {
                LogId = Guid.NewGuid(),
                EntityType = entityType,
                EntityId = entityId,
                Action = "Create",
                OldValue = null,
                NewValue = JsonSerializer.Serialize(newData),
                Role = user.RoleName,
                PerformedBy = userId,
                PerformedAt = DateTime.UtcNow
            };

            await _repo.CreateLogAsync(log);
        }

        public async Task LogUpdateAsync<T>(T oldData, T newData, Guid entityId, Guid userId, string entityType)
        {
            var user = await _usersRepo.GetUserShortInfoAsync(userId);
            var log = new AuditLog
            {
                LogId = Guid.NewGuid(),
                EntityType = entityType,
                EntityId = entityId,
                Action = "Update",
                OldValue = JsonSerializer.Serialize(oldData),
                NewValue = JsonSerializer.Serialize(newData),
                Role = user.RoleName,
                PerformedBy = userId,
                PerformedAt = DateTime.UtcNow
            };

            await _repo.CreateLogAsync(log);
        }

        public async Task LogSoftDeleteAsync<T>(T oldData, T newData, Guid entityId, Guid userId, string entityType)
        {
            var user = await _usersRepo.GetUserShortInfoAsync(userId);
            var log = new AuditLog
            {
                LogId = Guid.NewGuid(),
                EntityType = entityType,
                EntityId = entityId,
                Action = "SoftDelete",
                OldValue = JsonSerializer.Serialize(oldData),
                NewValue = JsonSerializer.Serialize(newData),
                Role = user.RoleName,
                PerformedBy = userId,
                PerformedAt = DateTime.UtcNow
            };

            await _repo.CreateLogAsync(log);
        }

        public async Task LogDeleteAsync<T>(T oldData, Guid entityId, Guid userId, string entityType)
        {
            var user = await _usersRepo.GetUserShortInfoAsync(userId);
            var log = new AuditLog
            {
                LogId = Guid.NewGuid(),
                EntityType = entityType,
                EntityId = entityId,
                Action = "Delete",
                OldValue = JsonSerializer.Serialize(oldData),
                NewValue = null,
                Role = user.RoleName,
                PerformedBy = userId,
                PerformedAt = DateTime.UtcNow
            };

            await _repo.CreateLogAsync(log);
        }

    }
}

