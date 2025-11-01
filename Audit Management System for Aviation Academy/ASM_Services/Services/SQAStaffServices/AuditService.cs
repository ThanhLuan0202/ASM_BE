using ASM_Repositories.Interfaces.SQAStaffInterfaces;
using ASM_Repositories.Models.AuditDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services.SQAStaffServices
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _repo;

        public AuditService(IAuditRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ViewAudit>> GetAllAuditAsync()
        {
            return await _repo.GetAllAuditAsync();
        }

        public async Task<ViewAudit?> GetAuditByIdAsync(Guid id)
        {
            return await _repo.GetAuditByIdAsync(id);
        }

        public async Task<ViewAudit> CreateAuditAsync(CreateAudit dto, Guid? createdByUserId)
        {
            return await _repo.CreateAuditAsync(dto, createdByUserId);
        }

        public async Task<ViewAudit?> UpdateAuditAsync(Guid id, UpdateAudit dto)
        {
            return await _repo.UpdateAuditAsync(id, dto);
        }

        public async Task<bool> DeleteAuditAsync(Guid id)
        {
            return await _repo.DeleteAuditAsync(id);
        }
    }
}
