using ASM_Repositories.Models.AuditCriteriaMapDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditCriteriaMapRepository
    {
        Task<IEnumerable<ViewAuditCriteriaMap>> GetByAuditIdAsync(Guid auditId);
        Task<ViewAuditCriteriaMap?> GetAsync(Guid auditId, Guid criteriaId);
        Task<ViewAuditCriteriaMap> CreateAsync(CreateAuditCriteriaMap dto);
        Task<bool> DeleteAsync(Guid auditId, Guid criteriaId);
        Task<bool> ExistsAsync(Guid auditId, Guid criteriaId);
    }
}
