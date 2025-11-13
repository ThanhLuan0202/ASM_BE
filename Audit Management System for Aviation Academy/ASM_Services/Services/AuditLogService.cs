using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditLogDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _repo;

        public AuditLogService(IAuditLogRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditLog>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditLog?> GetByIdAsync(Guid logId) => _repo.GetByIdAsync(logId);
        public Task<ViewAuditLog> CreateAsync(CreateAuditLog dto) => _repo.CreateAsync(dto);
        public Task<ViewAuditLog?> UpdateAsync(Guid logId, UpdateAuditLog dto) => _repo.UpdateAsync(logId, dto);
        public Task<bool> DeleteAsync(Guid logId) => _repo.DeleteAsync(logId);
    }
}

