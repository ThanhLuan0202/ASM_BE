using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditTeamDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
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
        private readonly IAuditLogService _logService;

        public AuditTeamService(IAuditTeamRepository repo, IUsersRepository userRepo, IAuditLogService logService)
        {
            _repo = repo;
            _userRepo = userRepo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewAuditTeam>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditTeam?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public async Task<ViewAuditTeam> CreateAsync(CreateAuditTeam dto, Guid userId)
        {
            var created = await _repo.AddAsync(dto);
            await _logService.LogCreateAsync(created, created.AuditTeamId, userId, "AuditTeam");
            return created;
        }
        public async Task<ViewAuditTeam?> UpdateAsync(Guid id, UpdateAuditTeam dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateAsync(id, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, id, userId, "AuditTeam");
            }
            return updated;
        }
        public async Task<bool> SoftDeleteAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var success = await _repo.SoftDeleteAsync(id);
            if (success && before != null)
            {
                await _logService.LogSoftDeleteAsync(before, before, id, userId, "AuditTeam");
            }
            return success;
        }
        public Task<bool> IsLeadAuditorAsync(Guid userId, Guid auditId) => _repo.IsLeadAuditorAsync(userId, auditId);
        public Task<IEnumerable<Guid>> GetAuditIdsByLeadAuditorAsync(Guid userId) => _repo.GetAuditIdsByLeadAuditorAsync(userId);
        public Task<IEnumerable<AuditorInfoDto>> GetAuditorsByAuditIdAsync(Guid auditId) => _repo.GetAuditorsByAuditIdAsync(auditId);

        public async Task<IEnumerable<AvailableTeamMemberDto>> GetAvailableTeamMembersAsync(bool excludePreviousPeriod = false, DateTime? previousPeriodStartDate = null, DateTime? previousPeriodEndDate = null)
        {
            var allUsers = await _userRepo.GetUsersByRolesAsync(new[] { "Auditor" });
            
            if (!excludePreviousPeriod)
            {
                return await MapWithLastAuditAsync(allUsers, previousPeriodStartDate, previousPeriodEndDate);
            }

            if (!previousPeriodStartDate.HasValue || !previousPeriodEndDate.HasValue)
            {
                return await MapWithLastAuditAsync(allUsers, previousPeriodStartDate, previousPeriodEndDate);
            }

            var usersInPreviousPeriod = await _repo.GetUsersInPeriodAsync(previousPeriodStartDate.Value, previousPeriodEndDate.Value);
            
            var availableUsers = allUsers.Where(u => !usersInPreviousPeriod.Contains(u.UserId)).ToList();
            
            return await MapWithLastAuditAsync(availableUsers, previousPeriodStartDate, previousPeriodEndDate);
        }

        private async Task<IEnumerable<AvailableTeamMemberDto>> MapWithLastAuditAsync(IEnumerable<UserAccount> users, DateTime? fromDate, DateTime? toDate)
        {
            var userList = users?.ToList() ?? new List<UserAccount>();
            if (!userList.Any()) return new List<AvailableTeamMemberDto>();

            var lastAudits = await _repo.GetLastAuditByUserIdsAsync(userList.Select(u => u.UserId), fromDate, toDate);

            return userList.Select(u => new AvailableTeamMemberDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.RoleName,
                LastAudit = lastAudits.TryGetValue(u.UserId, out var la) ? la : null
            }).ToList();
        }
    }
}
