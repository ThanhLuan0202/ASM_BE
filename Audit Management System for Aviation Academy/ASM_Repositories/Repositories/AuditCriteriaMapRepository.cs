using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditCriteriaMapDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditCriteriaMapRepository : Repository<AuditCriteriaMap>, IAuditCriteriaMapRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IMapper _mapper;

        public AuditCriteriaMapRepository(AuditManagementSystemForAviationAcademyContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditCriteriaMap>> GetByAuditIdAsync(Guid auditId)
        {
            var list = await _DbContext.AuditCriteriaMaps
                .Where(x => x.AuditId == auditId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ViewAuditCriteriaMap>>(list);
        }

        public async Task<ViewAuditCriteriaMap?> GetAsync(Guid auditId, Guid criteriaId)
        {
            var entity = await _DbContext.AuditCriteriaMaps.FindAsync(auditId, criteriaId);
            return entity == null ? null : _mapper.Map<ViewAuditCriteriaMap>(entity);
        }

        public async Task<ViewAuditCriteriaMap> CreateAsync(CreateAuditCriteriaMap dto)
        {
            var auditExists = await _DbContext.Audits.AnyAsync(a => a.AuditId == dto.AuditId);
            if (!auditExists) throw new InvalidOperationException($"Audit {dto.AuditId} not found");

            var criterionExists = await _DbContext.AuditCriteria.AnyAsync(c => c.CriteriaId == dto.CriteriaId);
            if (!criterionExists) throw new InvalidOperationException($"Criterion {dto.CriteriaId} not found");

            var exists = await _DbContext.AuditCriteriaMaps.AnyAsync(x => x.AuditId == dto.AuditId && x.CriteriaId == dto.CriteriaId);
            if (exists) throw new InvalidOperationException("Mapping already exists");

            var entity = _mapper.Map<AuditCriteriaMap>(dto);
            if (string.IsNullOrEmpty(entity.Status)) entity.Status = "Active";

            _DbContext.AuditCriteriaMaps.Add(entity);
            await _DbContext.SaveChangesAsync();
            return _mapper.Map<ViewAuditCriteriaMap>(entity);
        }

        public async Task<bool> DeleteAsync(Guid auditId, Guid criteriaId)
        {
            var entity = await _DbContext.AuditCriteriaMaps.FindAsync(auditId, criteriaId);
            if (entity == null) return false;

            _DbContext.AuditCriteriaMaps.Remove(entity);
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid auditId, Guid criteriaId)
        {
            return await _DbContext.AuditCriteriaMaps.AnyAsync(x => x.AuditId == auditId && x.CriteriaId == criteriaId);
        }
    }
}
