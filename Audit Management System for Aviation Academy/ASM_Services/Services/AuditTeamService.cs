using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditTeamDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditTeamService : IAuditTeamService
    {
        private readonly IAuditTeamRepository _repo;

        public AuditTeamService(IAuditTeamRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditTeam>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditTeam?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewAuditTeam> CreateAsync(CreateAuditTeam dto) => _repo.AddAsync(dto);
        public Task<ViewAuditTeam?> UpdateAsync(Guid id, UpdateAuditTeam dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> SoftDeleteAsync(Guid id) => _repo.SoftDeleteAsync(id);
        public Task<bool> IsLeadAuditorAsync(Guid userId, Guid auditId) => _repo.IsLeadAuditorAsync(userId, auditId);
        public Task<IEnumerable<Guid>> GetAuditIdsByLeadAuditorAsync(Guid userId) => _repo.GetAuditIdsByLeadAuditorAsync(userId);
    }
}
