using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ChecklistItemDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class ChecklistItemRepository : Repository<ChecklistItem>, IChecklistItemRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IMapper _mapper;

        public ChecklistItemRepository(AuditManagementSystemForAviationAcademyContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewChecklistItem>> GetAllChecklistItemAsync()
        {
            var items = await _DbContext.ChecklistItems
                .Include(i => i.Template)
                .Include(i => i.SeverityDefaultNavigation)
                .OrderBy(i => i.TemplateId)
                .ThenBy(i => i.Order)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewChecklistItem>>(items);
        }

        public async Task<IEnumerable<ViewChecklistItem>> GetChecklistItemsByTemplateIdAsync(Guid templateId)
        {
            var items = await _DbContext.ChecklistItems
                .Include(i => i.Template)
                .Include(i => i.SeverityDefaultNavigation)
                .Where(i => i.TemplateId == templateId)
                .OrderBy(i => i.Order)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewChecklistItem>>(items);
        }

        public async Task<ViewChecklistItem?> GetChecklistItemByIdAsync(Guid id)
        {
            var item = await _DbContext.ChecklistItems
                .Include(i => i.Template)
                .Include(i => i.SeverityDefaultNavigation)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            return item == null ? null : _mapper.Map<ViewChecklistItem>(item);
        }

        public async Task<ViewChecklistItem> CreateChecklistItemAsync(CreateChecklistItem dto)
        {
            var templateExists = await _DbContext.ChecklistTemplates.AnyAsync(t => t.TemplateId == dto.TemplateId);
            if (!templateExists)
            {
                throw new InvalidOperationException($"Template with ID {dto.TemplateId} does not exist");
            }

            if (!string.IsNullOrEmpty(dto.SeverityDefault))
            {
                var severityExists = await _DbContext.FindingSeverities.AnyAsync(s => s.Severity == dto.SeverityDefault);
                if (!severityExists)
                {
                    throw new InvalidOperationException($"Severity '{dto.SeverityDefault}' does not exist");
                }
            }

            var item = _mapper.Map<ChecklistItem>(dto);
            item.ItemId = Guid.NewGuid();

            _DbContext.ChecklistItems.Add(item);
            await _DbContext.SaveChangesAsync();

            var createdItem = await _DbContext.ChecklistItems
                .Include(i => i.Template)
                .Include(i => i.SeverityDefaultNavigation)
                .FirstOrDefaultAsync(i => i.ItemId == item.ItemId);

            return _mapper.Map<ViewChecklistItem>(createdItem);
        }

        public async Task<ViewChecklistItem?> UpdateChecklistItemAsync(Guid id, UpdateChecklistItem dto)
        {
            var existing = await _DbContext.ChecklistItems.FindAsync(id);
            if (existing == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(dto.SeverityDefault))
            {
                var severityExists = await _DbContext.FindingSeverities.AnyAsync(s => s.Severity == dto.SeverityDefault);
                if (!severityExists)
                {
                    throw new InvalidOperationException($"Severity '{dto.SeverityDefault}' does not exist");
                }
            }

            _mapper.Map(dto, existing);
            _DbContext.Entry(existing).State = EntityState.Modified;
            await _DbContext.SaveChangesAsync();

            var updatedItem = await _DbContext.ChecklistItems
                .Include(i => i.Template)
                .Include(i => i.SeverityDefaultNavigation)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            return _mapper.Map<ViewChecklistItem>(updatedItem);
        }

        public async Task<bool> DeleteChecklistItemAsync(Guid id)
        {
            var existing = await _DbContext.ChecklistItems.FindAsync(id);
            if (existing == null)
            {
                return false;
            }

            _DbContext.ChecklistItems.Remove(existing);
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _DbContext.ChecklistItems.AnyAsync(i => i.ItemId == id);
        }
    }
}
