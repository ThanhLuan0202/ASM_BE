using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.AuditStatusDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services.AdminServices
{
    public class AuditStatusService : IAuditStatusService
    {
        private readonly IAuditStatusRepository _repo;

        public AuditStatusService(IAuditStatusRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditStatus>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditStatus?> GetByIdAsync(string auditStatus) => _repo.GetByIdAsync(auditStatus);
        public Task<ViewAuditStatus> CreateAsync(CreateAuditStatus dto) => _repo.CreateAsync(dto);
        public Task<ViewAuditStatus?> UpdateAsync(string auditStatus, UpdateAuditStatus dto) => _repo.UpdateAsync(auditStatus, dto);
        public Task<bool> DeleteAsync(string auditStatus) => _repo.DeleteAsync(auditStatus);
    }
}

