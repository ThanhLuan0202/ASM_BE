using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Repositories.Models.AuditApprovalDTO;
using ASM_Services.Interfaces.DepartmentHeadInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services.DepartmentHeadServices
{
    public class AuditApprovalService : IAuditApprovalService
    {
        private readonly IAuditApprovalRepository _repo;

        public AuditApprovalService(IAuditApprovalRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditApproval>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditApproval?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewAuditApproval> CreateAsync(CreateAuditApproval dto) => _repo.AddAsync(dto);
        public Task<ViewAuditApproval> UpdateAsync(Guid id, UpdateAuditApproval dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> SoftDeleteAsync(Guid id, Guid userId) => _repo.SoftDeleteAsync(id, userId);
    }

}
