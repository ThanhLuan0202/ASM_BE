using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditTeamDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuditorInfoDto = ASM_Repositories.Models.AuditTeamDTO.AuditorInfoDto;

namespace ASM_Services.Services
{
    public class AuditTeamService : IAuditTeamService
    {
        private readonly IAuditTeamRepository _repo;
        private readonly IUsersRepository _userRepo;

        public AuditTeamService(IAuditTeamRepository repo, IUsersRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        public Task<IEnumerable<ViewAuditTeam>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditTeam?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewAuditTeam> CreateAsync(CreateAuditTeam dto) => _repo.AddAsync(dto);
        public Task<ViewAuditTeam?> UpdateAsync(Guid id, UpdateAuditTeam dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> SoftDeleteAsync(Guid id) => _repo.SoftDeleteAsync(id);
        public Task<bool> IsLeadAuditorAsync(Guid userId, Guid auditId) => _repo.IsLeadAuditorAsync(userId, auditId);
        public Task<IEnumerable<Guid>> GetAuditIdsByLeadAuditorAsync(Guid userId) => _repo.GetAuditIdsByLeadAuditorAsync(userId);
        public Task<IEnumerable<AuditorInfoDto>> GetAuditorsByAuditIdAsync(Guid auditId) => _repo.GetAuditorsByAuditIdAsync(auditId);

        public async Task<IEnumerable<UserAccount>> GetAvailableTeamMembersAsync(Guid currentAuditId, bool excludePreviousPeriod = false, DateTime? previousPeriodStartDate = null, DateTime? previousPeriodEndDate = null)
        {
            // Lấy tất cả users có role Auditor hoặc LeadAuditor
            var allUsers = await _userRepo.GetUsersByRolesAsync(new[] { "Auditor", "LeadAuditor" });
            
            if (!excludePreviousPeriod || !previousPeriodStartDate.HasValue || !previousPeriodEndDate.HasValue)
            {
                return allUsers;
            }

            // Lấy danh sách users đã tham gia audits trong thời kỳ trước
            var usersInPreviousPeriod = await _repo.GetUsersInPeriodAsync(previousPeriodStartDate.Value, previousPeriodEndDate.Value);
            
            // Loại bỏ những users đã tham gia thời kỳ trước
            var availableUsers = allUsers.Where(u => !usersInPreviousPeriod.Contains(u.UserId)).ToList();
            
            return availableUsers;
        }
    }
}
