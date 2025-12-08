using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditAssignmentDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditAssignmentService : IAuditAssignmentService
    {
        private readonly IAuditAssignmentRepository _repository;
        private readonly IAuditLogService _logService;

        public AuditAssignmentService(IAuditAssignmentRepository repository, IAuditLogService logService)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<IEnumerable<ViewAuditAssignment>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ViewAuditAssignment?> GetByIdAsync(Guid assignmentId)
        {
            return await _repository.GetByIdAsync(assignmentId);
        }

        public async Task<IEnumerable<ViewAuditAssignment>> GetByAuditIdAsync(Guid auditId)
        {
            return await _repository.GetByAuditIdAsync(auditId);
        }

        public async Task<IEnumerable<ViewAuditAssignment>> GetByAuditorIdAsync(Guid auditorId)
        {
            return await _repository.GetByAuditorIdAsync(auditorId);
        }

        public async Task<IEnumerable<ViewAuditAssignment>> GetByDeptIdAsync(int deptId)
        {
            return await _repository.GetByDeptIdAsync(deptId);
        }

        public async Task<ViewAuditAssignment> CreateAsync(CreateAuditAssignment dto, Guid userId)
        {
            var created = await _repository.CreateAsync(dto);
            await _logService.LogCreateAsync(created, created.AssignmentId, userId, "AuditAssignment");
            return created;
        }

        public async Task<ViewAuditAssignment?> UpdateAsync(Guid assignmentId, UpdateAuditAssignment dto, Guid userId)
        {
            var before = await _repository.GetByIdAsync(assignmentId);
            var updated = await _repository.UpdateAsync(assignmentId, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, assignmentId, userId, "AuditAssignment");
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(Guid assignmentId, Guid userId)
        {
            var before = await _repository.GetByIdAsync(assignmentId);
            var success = await _repository.DeleteAsync(assignmentId);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, assignmentId, userId, "AuditAssignment");
            }
            return success;
        }
    }
}

