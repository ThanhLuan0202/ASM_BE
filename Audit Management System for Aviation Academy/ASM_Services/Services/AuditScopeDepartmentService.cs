using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditScopeDepartmentService : IAuditScopeDepartmentService
    {
        private readonly IAuditScopeDepartmentRepository _repo;
        private readonly IAuditLogService _logService;

        public AuditScopeDepartmentService(IAuditScopeDepartmentRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewAuditScopeDepartment>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditScopeDepartment?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public async Task<ViewAuditScopeDepartment> CreateAsync(CreateAuditScopeDepartment dto, Guid userId)
        {
            var created = await _repo.AddAsync(dto);
            await _logService.LogCreateAsync(created, created.AuditScopeId, userId, "AuditScopeDepartment");
            return created;
        }
        public async Task<ViewAuditScopeDepartment?> UpdateAsync(Guid id, UpdateAuditScopeDepartment dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateAsync(id, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, id, userId, "AuditScopeDepartment");
            }
            return updated;
        }
        public async Task<bool> SoftDeleteAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var success = await _repo.SoftDeleteAsync(id);
            if (success && before != null)
            {
                await _logService.LogSoftDeleteAsync(before, before, id, userId, "AuditScopeDepartment");
            }
            return success;
        }
        public Task<IEnumerable<ViewDepartment>> GetDepartmentsByAuditIdAsync(Guid auditId) => _repo.GetDepartmentsByAuditIdAsync(auditId);
    }
}
