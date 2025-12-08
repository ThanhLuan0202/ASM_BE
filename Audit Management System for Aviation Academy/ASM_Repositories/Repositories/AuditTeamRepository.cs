using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditTeamDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LastAuditDto = ASM_Repositories.Models.AuditTeamDTO.LastAuditDto;

namespace ASM_Repositories.Repositories
{
    public class AuditTeamRepository : IAuditTeamRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditTeamRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditTeam>> GetAllAsync()
        {
            var list = await _context.AuditTeams
                .Where(x => x.Status == "Active")
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditTeam>>(list);
        }

        public async Task<ViewAuditTeam?> GetByIdAsync(Guid id)
        {
            var entity = await _context.AuditTeams
                .FirstOrDefaultAsync(x => x.AuditTeamId == id);

            return entity == null ? null : _mapper.Map<ViewAuditTeam>(entity);
        }

        public async Task<ViewAuditTeam> AddAsync(CreateAuditTeam dto)
        {
            try
            {
                if (!await _context.Audits.AnyAsync(a => a.AuditId == dto.AuditId))
                    throw new ArgumentException($"AuditId '{dto.AuditId}' does not exist.");
                if (!await _context.UserAccounts.AnyAsync(u => u.UserId == dto.UserId))
                    throw new ArgumentException($"UserId '{dto.UserId}' does not exist.");

                bool duplicate = await _context.AuditTeams
                    .AnyAsync(x => x.AuditId == dto.AuditId && x.UserId == dto.UserId);
                if (duplicate)
                    throw new ArgumentException("This user is already assigned to the audit.");

                if (dto.IsLead)
                {
                    bool hasLead = await _context.AuditTeams
                        .AnyAsync(x => x.AuditId == dto.AuditId && x.IsLead && x.Status == "Active");
                    if (hasLead)
                        throw new ArgumentException("This audit already has a lead assigned.");
                }

                var entity = _mapper.Map<AuditTeam>(dto);
                _context.AuditTeams.Add(entity);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewAuditTeam>(entity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AuditTeamRepository.AddAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while creating AuditTeam.", ex);
            }
        }

        public async Task<ViewAuditTeam?> UpdateAsync(Guid id, UpdateAuditTeam dto)
        {
            var entity = await _context.AuditTeams.FirstOrDefaultAsync(x => x.AuditTeamId == id);
            if (entity == null) return null;

            if (dto.IsLead.HasValue && dto.IsLead.Value)
            {
                bool hasOtherLead = await _context.AuditTeams
                    .AnyAsync(x => x.AuditId == entity.AuditId && x.IsLead && x.AuditTeamId != id);
                if (hasOtherLead)
                    throw new ArgumentException("Another lead already exists in this audit.");
            }

            _mapper.Map(dto, entity);
            _context.AuditTeams.Update(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAuditTeam>(entity);
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.AuditTeams.FirstOrDefaultAsync(x => x.AuditTeamId == id);
            if (entity == null || entity.Status == "Inactive") return false;

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateAuditTeamsAsync(Guid auditId, List<UpdateAuditTeam>? list)
        {
            if (list == null || !list.Any())
                return; // Không có gì để update, bỏ qua

            // Xóa team cũ
            var existing = _context.AuditTeams
                .Where(x => x.AuditId == auditId);
            _context.AuditTeams.RemoveRange(existing);

            // Thêm team mới
            foreach (var item in list)
            {
                var entity = _mapper.Map<AuditTeam>(item);
                entity.AuditId = auditId;
                await _context.AuditTeams.AddAsync(entity);
            }
        }
        /*
        public async Task<Guid?> GetLeadUserIdByAuditIdAsync(Guid auditId)
        {
            var lead = await _context.AuditTeams
                .Where(t => t.AuditId == auditId && t.IsLead == true)
                .Select(t => t.UserId)
                .FirstOrDefaultAsync();

            return lead == Guid.Empty ? null : lead;
        }
        */
        public async Task<bool> IsLeadAuditorAsync(Guid userId, Guid auditId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty");

            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty");

            var isLeadAuditor = await _context.AuditTeams
                .AnyAsync(t => t.UserId == userId 
                    && t.AuditId == auditId 
                    && t.RoleInTeam == "LeadAuditor" 
                    && t.Status == "Active");

            return isLeadAuditor;
        }

        public async Task<IEnumerable<Guid>> GetAuditIdsByLeadAuditorAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty");

            var auditIds = await _context.AuditTeams
                .Where(t => t.UserId == userId 
                    && t.RoleInTeam == "LeadAuditor" 
                    && t.Status == "Active")
                .Select(t => t.AuditId)
                .Distinct()
                .ToListAsync();

            return auditIds;
        }

        public async Task<IEnumerable<Models.AuditTeamDTO.AuditorInfoDto>> GetAuditorsByAuditIdAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty");

            var auditors = await _context.AuditTeams
                .Where(t => t.AuditId == auditId 
                    && t.RoleInTeam == "Auditor" 
                    && t.Status == "Active")
                .Include(t => t.User)
                .Select(t => new Models.AuditTeamDTO.AuditorInfoDto
                {
                    UserId = t.UserId,
                    FullName = t.User != null ? t.User.FullName : string.Empty,
                    Email = t.User != null ? t.User.Email : string.Empty
                })
                .Distinct()
                .OrderBy(a => a.FullName)
                .ToListAsync();

            return auditors;
        }

        public async Task UpdateStatusToArchivedAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty.");

            var entities = await _context.AuditTeams
                .Where(a => a.AuditId == auditId)
                .ToListAsync();

            if (entities.Count == 0)
                throw new InvalidOperationException($"No AuditTeam found for AuditId '{auditId}'.");

            foreach (var entity in entities)
            {
                entity.Status = "Archived";
                _context.Entry(entity).Property(x => x.Status).IsModified = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetUsersInPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var userIds = await _context.AuditTeams
                .Include(at => at.Audit)
                .Where(at => at.Audit.StartDate >= startDate 
                    && at.Audit.EndDate <= endDate
                    && at.Status == "Active"
                    && at.Audit.Status != "Inactive")
                .Select(at => at.UserId)
                .Distinct()
                .ToListAsync();

            return userIds;
        }

        public async Task<Dictionary<Guid, LastAuditDto>> GetLastAuditByUserIdsAsync(IEnumerable<Guid> userIds, DateTime? fromDate, DateTime? toDate)
        {
            var idList = userIds?.Distinct().ToList() ?? new List<Guid>();
            if (!idList.Any())
                return new Dictionary<Guid, LastAuditDto>();

            var query = _context.AuditTeams
                .Include(at => at.Audit)
                .Where(at => idList.Contains(at.UserId)
                    && at.Status == "Active"
                    && at.Audit != null
                    && at.Audit.Status != "Inactive");

            if (fromDate.HasValue)
            {
                query = query.Where(at => (at.Audit.StartDate ?? DateTime.MinValue) >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(at => (at.Audit.EndDate ?? at.Audit.StartDate ?? DateTime.MaxValue) <= toDate.Value);
            }

            var auditTeams = await query
                .OrderBy(at => at.UserId)
                .ThenByDescending(at => at.Audit.EndDate ?? at.Audit.StartDate ?? DateTime.MinValue)
                .ToListAsync();

            var result = new Dictionary<Guid, LastAuditDto>();

            foreach (var at in auditTeams)
            {
                if (result.ContainsKey(at.UserId)) continue;

                result[at.UserId] = new LastAuditDto
                {
                    AuditId = at.AuditId,
                    Title = at.Audit?.Title,
                    StartDate = at.Audit?.StartDate,
                    EndDate = at.Audit?.EndDate
                };
            }

            return result;
        }
    }

}
