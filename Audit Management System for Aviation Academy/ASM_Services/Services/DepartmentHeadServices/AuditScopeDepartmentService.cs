using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using ASM_Services.Interfaces.DepartmentHeadInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services.DepartmentHeadServices
{
    public class AuditScopeDepartmentService : IAuditScopeDepartmentService
    {
        private readonly IAuditScopeDepartmentRepository _repo;

        public AuditScopeDepartmentService(IAuditScopeDepartmentRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditScopeDepartment>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditScopeDepartment?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewAuditScopeDepartment> CreateAsync(CreateAuditScopeDepartment dto) => _repo.AddAsync(dto);
        public Task<ViewAuditScopeDepartment?> UpdateAsync(Guid id, UpdateAuditScopeDepartment dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> SoftDeleteAsync(Guid id) => _repo.SoftDeleteAsync(id);
    }
}
