using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditCriteriaMapDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditCriteriaMapService : IAuditCriteriaMapService
    {
        private readonly IAuditCriteriaMapRepository _repo;
        private readonly IAuditLogService _logService;

        public AuditCriteriaMapService(IAuditCriteriaMapRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public async Task<IEnumerable<ViewAuditCriteriaMap>> GetByAuditIdAsync(Guid auditId)
        {
            return await _repo.GetByAuditIdAsync(auditId);
        }

        public async Task<ViewAuditCriteriaMap?> GetAsync(Guid auditId, Guid criteriaId)
        {
            return await _repo.GetAsync(auditId, criteriaId);
        }

        public async Task<ViewAuditCriteriaMap> CreateAsync(CreateAuditCriteriaMap dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            await _logService.LogCreateAsync(created, created.AuditId, userId, "AuditCriteriaMap");
            return created;
        }

        public async Task<bool> DeleteAsync(Guid auditId, Guid criteriaId, Guid userId)
        {
            var before = await _repo.GetAsync(auditId, criteriaId);
            var success = await _repo.DeleteAsync(auditId, criteriaId);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, auditId, userId, "AuditCriteriaMap");
            }
            return success;
        }
    }
}
