using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditCriteriaMapDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditCriteriaMapService : IAuditCriteriaMapService
    {
        private readonly IAuditCriteriaMapRepository _repo;

        public AuditCriteriaMapService(IAuditCriteriaMapRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ViewAuditCriteriaMap>> GetByAuditIdAsync(Guid auditId)
        {
            return await _repo.GetByAuditIdAsync(auditId);
        }

        public async Task<ViewAuditCriteriaMap?> GetAsync(Guid auditId, Guid criteriaId)
        {
            return await _repo.GetAsync(auditId, criteriaId);
        }

        public async Task<ViewAuditCriteriaMap> CreateAsync(CreateAuditCriteriaMap dto)
        {
            return await _repo.CreateAsync(dto);
        }

        public async Task<bool> DeleteAsync(Guid auditId, Guid criteriaId)
        {
            return await _repo.DeleteAsync(auditId, criteriaId);
        }
    }
}
