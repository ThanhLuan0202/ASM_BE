using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.RootCauseDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class RootCauseRepository : Repository<RootCause>, IRootCauseRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IMapper _mapper;

        public RootCauseRepository(AuditManagementSystemForAviationAcademyContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewRootCause>> GetAllAsync()
        {
            var list = await _DbContext.RootCauses.ToListAsync();
            return _mapper.Map<IEnumerable<ViewRootCause>>(list);
        }

        public async Task<ViewRootCause?> GetByIdAsync(int id)
        {
            var entity = await _DbContext.RootCauses.FindAsync(id);
            return entity == null ? null : _mapper.Map<ViewRootCause>(entity);
        }

        public async Task<ViewRootCause> CreateAsync(CreateRootCause dto)
        {
            var entity = _mapper.Map<RootCause>(dto);
            _DbContext.RootCauses.Add(entity);
            await _DbContext.SaveChangesAsync();
            return _mapper.Map<ViewRootCause>(entity);
        }

        public async Task<ViewRootCause?> UpdateAsync(int id, UpdateRootCause dto)
        {
            var existing = await _DbContext.RootCauses.FindAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _DbContext.SaveChangesAsync();
            return _mapper.Map<ViewRootCause>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _DbContext.RootCauses.FindAsync(id);
            if (existing == null) return false;

            existing.Status = "Inactive";
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _DbContext.RootCauses.AnyAsync(x => x.RootCauseId == id);
        }

        public async Task<Dictionary<int, string>> GetRootCausesAsync(List<int> rootIds)
        => await _DbContext.RootCauses.Where(r => rootIds.Contains(r.RootCauseId))
            .ToDictionaryAsync(r => r.RootCauseId, r => r.Name);
    }
}
