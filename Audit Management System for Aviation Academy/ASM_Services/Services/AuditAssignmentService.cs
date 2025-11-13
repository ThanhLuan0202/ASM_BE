using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditAssignmentDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditAssignmentService : IAuditAssignmentService
    {
        private readonly IAuditAssignmentRepository _repository;

        public AuditAssignmentService(IAuditAssignmentRepository repository)
        {
            _repository = repository;
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

        public async Task<ViewAuditAssignment> CreateAsync(CreateAuditAssignment dto)
        {
            return await _repository.CreateAsync(dto);
        }

        public async Task<ViewAuditAssignment?> UpdateAsync(Guid assignmentId, UpdateAuditAssignment dto)
        {
            return await _repository.UpdateAsync(assignmentId, dto);
        }

        public async Task<bool> DeleteAsync(Guid assignmentId)
        {
            return await _repository.DeleteAsync(assignmentId);
        }
    }
}

