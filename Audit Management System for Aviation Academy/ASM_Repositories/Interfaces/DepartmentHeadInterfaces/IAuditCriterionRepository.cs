using ASM_Repositories.Models.AuditCriterionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.DepartmentHeadInterfaces
{
    public interface IAuditCriterionRepository
    {
        Task<IEnumerable<ViewAuditCriterion>> GetAllAsync();
        Task<ViewAuditCriterion?> GetByIdAsync(Guid id);
        Task<ViewAuditCriterion> CreateAsync(CreateAuditCriterion dto);
        Task<ViewAuditCriterion?> UpdateAsync(Guid id, UpdateAuditCriterion dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}
