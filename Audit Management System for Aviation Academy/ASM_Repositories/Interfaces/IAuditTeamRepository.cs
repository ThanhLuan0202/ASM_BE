using ASM_Repositories.Models.AuditTeamDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuditorInfoDto = ASM_Repositories.Models.AuditTeamDTO.AuditorInfoDto;
using LastAuditDto = ASM_Repositories.Models.AuditTeamDTO.LastAuditDto;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditTeamRepository
    {
        Task<IEnumerable<ViewAuditTeam>> GetAllAsync();
        Task<ViewAuditTeam?> GetByIdAsync(Guid id);
        Task<ViewAuditTeam> AddAsync(CreateAuditTeam dto);
        Task<ViewAuditTeam?> UpdateAsync(Guid id, UpdateAuditTeam dto);
        Task<bool> SoftDeleteAsync(Guid id);
        Task UpdateAuditTeamsAsync(Guid auditId, List<UpdateAuditTeam>? list);
        //Task<Guid?> GetLeadUserIdByAuditIdAsync(Guid auditId);
        Task<bool> IsLeadAuditorAsync(Guid userId, Guid auditId);
        Task<IEnumerable<Guid>> GetAuditIdsByLeadAuditorAsync(Guid userId);
        Task<IEnumerable<AuditorInfoDto>> GetAuditorsByAuditIdAsync(Guid auditId);
        Task UpdateStatusToArchivedAsync(Guid auditId);
        Task<List<Guid>> GetUsersInPeriodAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<Guid, LastAuditDto>> GetLastAuditByUserIdsAsync(IEnumerable<Guid> userIds, DateTime? fromDate, DateTime? toDate);
    }
}
