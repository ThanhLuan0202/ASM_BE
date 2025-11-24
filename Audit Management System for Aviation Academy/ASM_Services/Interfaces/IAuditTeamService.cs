using ASM_Repositories.Models.AuditTeamDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IAuditTeamService
    {
        Task<IEnumerable<ViewAuditTeam>> GetAllAsync();
        Task<ViewAuditTeam?> GetByIdAsync(Guid id);
        Task<ViewAuditTeam> CreateAsync(CreateAuditTeam dto);
        Task<ViewAuditTeam?> UpdateAsync(Guid id, UpdateAuditTeam dto);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<bool> IsLeadAuditorAsync(Guid userId, Guid auditId);
        Task<IEnumerable<Guid>> GetAuditIdsByLeadAuditorAsync(Guid userId);
    }
}
