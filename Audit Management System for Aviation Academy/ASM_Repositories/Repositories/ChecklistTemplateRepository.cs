using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ChecklistTemplateDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class ChecklistTemplateRepository : Repository<ChecklistTemplate>, IChecklistTemplateRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IMapper _mapper;

        public ChecklistTemplateRepository(AuditManagementSystemForAviationAcademyContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewChecklistTemplate>> GetAllChecklistTemplateAsync()
        {
            var templates = await _DbContext.ChecklistTemplates
                .Include(t => t.CreatedByNavigation)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewChecklistTemplate>>(templates);
        }

        public async Task<ViewChecklistTemplate?> GetChecklistTemplateByIdAsync(Guid id)
        {
            var template = await _DbContext.ChecklistTemplates
                .Include(t => t.CreatedByNavigation)
                .FirstOrDefaultAsync(t => t.TemplateId == id);

            return template == null ? null : _mapper.Map<ViewChecklistTemplate>(template);
        }

        public async Task<ViewChecklistTemplate> CreateChecklistTemplateAsync(CreateChecklistTemplate dto, Guid? createdByUserId)
        {
            if (createdByUserId.HasValue)
            {
                var userExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == createdByUserId.Value);
                if (!userExists)
                {
                    throw new InvalidOperationException($"User with ID {createdByUserId} does not exist");
                }
            }

            if (!string.IsNullOrEmpty(dto.Status))
            {
                
            }

            var template = _mapper.Map<ChecklistTemplate>(dto);
            template.TemplateId = Guid.NewGuid();
            template.CreatedAt = DateTime.UtcNow;
            template.CreatedBy = createdByUserId;
            template.IsActive = dto.IsActive;

            _DbContext.ChecklistTemplates.Add(template);
            await _DbContext.SaveChangesAsync();

            var createdTemplate = await _DbContext.ChecklistTemplates
                .Include(t => t.CreatedByNavigation)
                .FirstOrDefaultAsync(t => t.TemplateId == template.TemplateId);

            return _mapper.Map<ViewChecklistTemplate>(createdTemplate);
        }

        public async Task<ViewChecklistTemplate?> UpdateChecklistTemplateAsync(Guid id, UpdateChecklistTemplate dto)
        {
            var existing = await _DbContext.ChecklistTemplates.FindAsync(id);
            if (existing == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Status))
            {
            }

            _mapper.Map(dto, existing);
            await _DbContext.SaveChangesAsync();

            var updatedTemplate = await _DbContext.ChecklistTemplates
                .Include(t => t.CreatedByNavigation)
                .FirstOrDefaultAsync(t => t.TemplateId == id);

            return _mapper.Map<ViewChecklistTemplate>(updatedTemplate);
        }

        public async Task<bool> DeleteChecklistTemplateAsync(Guid id)
        {
            var existing = await _DbContext.ChecklistTemplates.FindAsync(id);
            if (existing == null)
            {
                return false;
            }

            existing.IsActive = false;
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _DbContext.ChecklistTemplates.AnyAsync(t => t.TemplateId == id);
        }
    }
}
