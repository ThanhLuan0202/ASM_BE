using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditPlanAssignmentService : IAuditPlanAssignmentService
    {
        private readonly IAuditPlanAssignmentRepository _repo;

        public AuditPlanAssignmentService(IAuditPlanAssignmentRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditPlanAssignment>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditPlanAssignment?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewAuditPlanAssignment> CreateAsync(CreateAuditPlanAssignment dto) => _repo.CreateAsync(dto);
        public Task<ViewAuditPlanAssignment?> UpdateAsync(Guid id, UpdateAuditPlanAssignment dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> DeleteAsync(Guid id) => _repo.DeleteAsync(id);
    }
}
