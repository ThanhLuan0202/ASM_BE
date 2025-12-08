using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditApprovalDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditApprovalService : IAuditApprovalService
    {
        private readonly IAuditApprovalRepository _repo;
        private readonly IAuditLogService _logService;

        public AuditApprovalService(IAuditApprovalRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewAuditApproval>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditApproval?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public async Task<ViewAuditApproval> CreateAsync(CreateAuditApproval dto, Guid userId)
        {
            var created = await _repo.AddAsync(dto);
            await _logService.LogCreateAsync(created, created.ApprovalId, userId, "AuditApproval");
            return created;
        }
        public async Task<ViewAuditApproval> UpdateAsync(Guid id, UpdateAuditApproval dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateAsync(id, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, id, userId, "AuditApproval");
            }
            return updated;
        }
        public async Task<bool> SoftDeleteAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var success = await _repo.SoftDeleteAsync(id, userId);
            if (success && before != null)
            {
                await _logService.LogSoftDeleteAsync(before, before, id, userId, "AuditApproval");
            }
            return success;
        }
    }

}
