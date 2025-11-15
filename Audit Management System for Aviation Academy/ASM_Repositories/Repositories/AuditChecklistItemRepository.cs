using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditChecklistItemDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditChecklistItemRepository : Repository<AuditChecklistItem>, IAuditChecklistItemRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IMapper _mapper;

        public AuditChecklistItemRepository(AuditManagementSystemForAviationAcademyContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetByAuditIdAsync(Guid auditId)
        {
            var list = await _DbContext.AuditChecklistItems
                .Where(x => x.AuditId == auditId)
                .OrderBy(x => x.Section)
                .ThenBy(x => x.Order)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ViewAuditChecklistItem>>(list);
        }

        public async Task<ViewAuditChecklistItem?> GetByIdAsync(Guid auditItemId)
        {
            var entity = await _DbContext.AuditChecklistItems.FindAsync(auditItemId);
            return entity == null ? null : _mapper.Map<ViewAuditChecklistItem>(entity);
        }

        public async Task<ViewAuditChecklistItem> CreateAsync(CreateAuditChecklistItem dto)
        {
            var auditExists = await _DbContext.Audits.AnyAsync(a => a.AuditId == dto.AuditId);
            if (!auditExists) throw new InvalidOperationException($"Audit {dto.AuditId} not found");

            var entity = _mapper.Map<AuditChecklistItem>(dto);
            entity.AuditItemId = Guid.NewGuid();

            _DbContext.AuditChecklistItems.Add(entity);
            await _DbContext.SaveChangesAsync();
            return _mapper.Map<ViewAuditChecklistItem>(entity);
        }

        public async Task<ViewAuditChecklistItem?> UpdateAsync(Guid auditItemId, UpdateAuditChecklistItem dto)
        {
            var existing = await _DbContext.AuditChecklistItems.FindAsync(auditItemId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _DbContext.SaveChangesAsync();
            return _mapper.Map<ViewAuditChecklistItem>(existing);
        }

        public async Task<bool> DeleteAsync(Guid auditItemId)
        {
            var existing = await _DbContext.AuditChecklistItems.FindAsync(auditItemId);
            if (existing == null) return false;

            _DbContext.AuditChecklistItems.Remove(existing);
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetBySectionAsync(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
                throw new ArgumentException("Section cannot be null or empty.");

            var isValidSection = await _DbContext.Departments
                .AnyAsync(d => d.Name == section && d.Status == "Active");

            if (!isValidSection)
                throw new InvalidOperationException($"Section '{section}' is not a valid department name or the department is not active.");

            var items = await _DbContext.AuditChecklistItems
                .Where(aci => aci.Section == section)
                .OrderBy(aci => aci.Order)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditChecklistItem>>(items);
        }
    }
}
