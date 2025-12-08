using ASM_Repositories.Models.AuditCriterionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IAuditCriterionService
    {
        Task<IEnumerable<ViewAuditCriterion>> GetAllAsync();
        Task<ViewAuditCriterion?> GetByIdAsync(Guid id);
        Task<ViewAuditCriterion> CreateAsync(CreateAuditCriterion dto, Guid userId);
        Task<ViewAuditCriterion?> UpdateAsync(Guid id, UpdateAuditCriterion dto, Guid userId);
        Task<bool> SoftDeleteAsync(Guid id, Guid userId);
    }
}
