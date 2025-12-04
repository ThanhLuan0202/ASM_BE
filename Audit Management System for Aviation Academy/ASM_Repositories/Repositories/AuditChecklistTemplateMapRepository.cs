using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditChecklistTemplateMapDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditChecklistTemplateMapRepository : IAuditChecklistTemplateMapRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditChecklistTemplateMapRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditChecklistTemplateMap>> GetAllAsync()
        {
            var query = _context.AuditChecklistTemplateMaps.AsNoTracking();
            var entities = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ViewAuditChecklistTemplateMap>>(entities);
        }

        public async Task<ViewAuditChecklistTemplateMap> GetAsync(Guid auditId, Guid templateId)
        {
            var entity = await _context.AuditChecklistTemplateMaps
                .FirstOrDefaultAsync(x => x.AuditId == auditId && x.TemplateId == templateId);

            return entity == null ? null : _mapper.Map<ViewAuditChecklistTemplateMap>(entity);
        }

        public async Task<ViewAuditChecklistTemplateMap> CreateAsync(CreateAuditChecklistTemplateMap dto)
        {
            var entity = _mapper.Map<AuditChecklistTemplateMap>(dto);

            _context.AuditChecklistTemplateMaps.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAuditChecklistTemplateMap>(entity);
        }

        public async Task<ViewAuditChecklistTemplateMap> UpdateAsync(
            Guid auditId,
            Guid templateId,
            UpdateAuditChecklistTemplateMap dto)
        {
            var entity = await _context.AuditChecklistTemplateMaps
                .FirstOrDefaultAsync(x => x.AuditId == auditId && x.TemplateId == templateId);

            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAuditChecklistTemplateMap>(entity);
        }

        public async Task DeleteAsync(Guid auditId, Guid templateId)
        {
            var entity = await _context.AuditChecklistTemplateMaps
                .FirstOrDefaultAsync(x => x.AuditId == auditId && x.TemplateId == templateId);

            if (entity == null) return;

            entity.Status = "Inactive";
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }

}
