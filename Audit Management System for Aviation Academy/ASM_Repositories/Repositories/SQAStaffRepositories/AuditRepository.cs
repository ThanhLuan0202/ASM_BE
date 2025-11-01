using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.SQAStaffInterfaces;
using ASM_Repositories.Models.AuditDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.SQAStaffRepositories
{
    public class AuditRepository : Repository<Audit>, IAuditRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IMapper _mapper;

        public AuditRepository(AuditManagementSystemForAviationAcademyContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAudit>> GetAllAuditAsync()
        {
            var audits = await _DbContext.Audits
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.Template)
                .Include(a => a.StatusNavigation)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAudit>>(audits);
        }

        public async Task<ViewAudit?> GetAuditByIdAsync(Guid id)
        {
            var audit = await _DbContext.Audits
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.Template)
                .Include(a => a.StatusNavigation)
                .FirstOrDefaultAsync(a => a.AuditId == id);

            return audit == null ? null : _mapper.Map<ViewAudit>(audit);
        }

        public async Task<ViewAudit> CreateAuditAsync(CreateAudit dto, Guid? createdByUserId)
        {
            if (dto.TemplateId.HasValue)
            {
                var templateExists = await _DbContext.ChecklistTemplates.AnyAsync(t => t.TemplateId == dto.TemplateId.Value);
                if (!templateExists)
                {
                    throw new InvalidOperationException($"Template with ID {dto.TemplateId} does not exist");
                }
            }

            if (createdByUserId.HasValue)
            {
                var userExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == createdByUserId.Value);
                if (!userExists)
                {
                    throw new InvalidOperationException($"User with ID {createdByUserId} does not exist");
                }
            }

            string status = dto.Status ?? "Draft";
            if (!string.IsNullOrEmpty(status))
            {
                var statusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == status);
                if (!statusExists)
                {
                    throw new InvalidOperationException($"Status '{status}' does not exist");
                }
            }

            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate.Value > dto.EndDate.Value)
            {
                throw new InvalidOperationException("StartDate cannot be later than EndDate");
            }

            var audit = _mapper.Map<Audit>(dto);
            audit.AuditId = Guid.NewGuid();
            audit.CreatedAt = DateTime.UtcNow;
            audit.Status = status;
            audit.CreatedBy = createdByUserId; 

            _DbContext.Audits.Add(audit);
            await _DbContext.SaveChangesAsync();

            var createdAudit = await _DbContext.Audits
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.Template)
                .Include(a => a.StatusNavigation)
                .FirstOrDefaultAsync(a => a.AuditId == audit.AuditId);

            return _mapper.Map<ViewAudit>(createdAudit);
        }

        public async Task<ViewAudit?> UpdateAuditAsync(Guid id, UpdateAudit dto)
        {
            var existing = await _DbContext.Audits.FindAsync(id);
            if (existing == null)
            {
                return null;
            }

            if (dto.TemplateId.HasValue)
            {
                var templateExists = await _DbContext.ChecklistTemplates.AnyAsync(t => t.TemplateId == dto.TemplateId.Value);
                if (!templateExists)
                {
                    throw new InvalidOperationException($"Template with ID {dto.TemplateId} does not exist");
                }
            }

            if (!string.IsNullOrEmpty(dto.Status))
            {
                var statusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == dto.Status);
                if (!statusExists)
                {
                    throw new InvalidOperationException($"Status '{dto.Status}' does not exist");
                }
            }

            if (dto.StartDate.HasValue && dto.EndDate.HasValue && dto.StartDate.Value > dto.EndDate.Value)
            {
                throw new InvalidOperationException("StartDate cannot be later than EndDate");
            }

            _mapper.Map(dto, existing);
            await _DbContext.SaveChangesAsync();

            var updatedAudit = await _DbContext.Audits
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.Template)
                .Include(a => a.StatusNavigation)
                .FirstOrDefaultAsync(a => a.AuditId == id);

            return _mapper.Map<ViewAudit>(updatedAudit);
        }

        public async Task<bool> DeleteAuditAsync(Guid id)
        {
            var existing = await _DbContext.Audits.FindAsync(id);
            if (existing == null)
            {
                return false;
            }

            var inactiveStatusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == "Inactive");
            if (!inactiveStatusExists)
            {
                throw new InvalidOperationException("Status 'Inactive' does not exist in the system. Please add it to AuditStatus table first.");
            }

            existing.Status = "Inactive";
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _DbContext.Audits.AnyAsync(a => a.AuditId == id);
        }
    }
}

