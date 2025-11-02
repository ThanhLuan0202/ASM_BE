using ASM_Repositories.Models.AuditTeamDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.DepartmentHeadInterfaces
{
    public interface IAuditTeamRepository
    {
        Task<IEnumerable<ViewAuditTeam>> GetAllAsync();
        Task<ViewAuditTeam?> GetByIdAsync(Guid id);
        Task<ViewAuditTeam> AddAsync(CreateAuditTeam dto);
        Task<ViewAuditTeam?> UpdateAsync(Guid id, UpdateAuditTeam dto);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}
