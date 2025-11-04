using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Repositories.Models.AuditTeamDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.DepartmentHeadRepositories
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
    }

}
