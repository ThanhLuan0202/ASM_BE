using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
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

        public async Task<List<ViewAuditPlan>> GetAllAuditPlansAsync()
        {
            var audits = await _DbContext.Audits
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.AuditScopeDepartments).ThenInclude(x => x.Dept)
                .Include(a => a.AuditCriteriaMaps).ThenInclude(x => x.Criteria)
                .Include(a => a.AuditTeams).ThenInclude(x => x.User)
                .Include(a => a.AuditSchedules)
                .ToListAsync();

            return _mapper.Map<List<ViewAuditPlan>>(audits);
        }

        public async Task<ViewAuditPlan?> GetAuditPlanByIdAsync(Guid auditId)
        {
            var audit = await _DbContext.Audits
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.AuditScopeDepartments).ThenInclude(x => x.Dept)
                .Include(a => a.AuditCriteriaMaps).ThenInclude(x => x.Criteria)
                .Include(a => a.AuditTeams).ThenInclude(x => x.User)
                .Include(a => a.AuditSchedules)
                .FirstOrDefaultAsync(a => a.AuditId == auditId);

            return audit == null ? null : _mapper.Map<ViewAuditPlan>(audit);
        }

        public async Task<bool> UpdateStatusAsync(Guid auditId, string status)
        {
            var existing = await _DbContext.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);
            if (existing == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status is required");
            }

            var statusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == status);
            if (!statusExists)
            {
                throw new InvalidOperationException($"Status '{status}' does not exist");
            }

            existing.Status = status;
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SubmitToLeadAuditorAsync(Guid auditId)
        {
            var audit = await _DbContext.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);
            if (audit == null)
            {
                return false;
            }

            var statusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == "PendingReview");
            if (!statusExists)
            {
                throw new InvalidOperationException("Status 'PendingReview' does not exist in AuditStatus");
            }

            audit.Status = "PendingReview";

            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectPlanContentAsync(Guid auditId, Guid approverId, string comment)
        {
            var audit = await _DbContext.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);
            if (audit == null)
            {
                return false;
            }

            var approverExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == approverId);
            if (!approverExists)
            {
                throw new InvalidOperationException($"ApproverId '{approverId}' does not exist");
            }

            var rejectedStatusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == "Rejected");
            if (!rejectedStatusExists)
            {
                throw new InvalidOperationException("Status 'Rejected' does not exist in AuditStatus");
            }

            audit.Status = "Rejected";

            var approval = new AuditApproval
            {
                AuditApprovalId = Guid.NewGuid(),
                AuditId = auditId,
                ApproverId = approverId,
                ApprovalLevel = "Lead Auditor",
                Status = "Rejected Plan",
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                ApprovedAt = DateTime.UtcNow
            };
            _DbContext.AuditApprovals.Add(approval);

            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveAndForwardToDirectorAsync(Guid auditId, Guid approverId, string comment)
        {
            var audit = await _DbContext.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);
            if (audit == null)
            {
                return false;
            }

            var approverExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == approverId);
            if (!approverExists)
            {
                throw new InvalidOperationException($"ApproverId '{approverId}' does not exist");
            }

            var pendingDirExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == "PendingDirectorApproval");
            if (!pendingDirExists)
            {
                throw new InvalidOperationException("Status 'PendingDirectorApproval' does not exist in AuditStatus");
            }

            audit.Status = "PendingDirectorApproval";

            var approval = new AuditApproval
            {
                AuditApprovalId = Guid.NewGuid(),
                AuditId = auditId,
                ApproverId = approverId,
                ApprovalLevel = "Lead Auditor",
                Status = "PendingDirectorApproval",
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                ApprovedAt = DateTime.UtcNow
            };
            _DbContext.AuditApprovals.Add(approval);

            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApprovePlanAsync(Guid auditId, Guid approverId, string comment)
        {
            var audit = await _DbContext.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);
            if (audit == null)
            {
                return false;
            }

            var approverExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == approverId);
            if (!approverExists)
            {
                throw new InvalidOperationException($"ApproverId '{approverId}' does not exist");
            }

            var approveStatusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == "Approve");
            if (!approveStatusExists)
            {
                throw new InvalidOperationException("Status 'Approve' does not exist in AuditStatus");
            }

            audit.Status = "Approve";

            var approval = new AuditApproval
            {
                AuditApprovalId = Guid.NewGuid(),
                AuditId = auditId,
                ApproverId = approverId,
                ApprovalLevel = "Director",
                Status = "Approved",
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                ApprovedAt = DateTime.UtcNow
            };
            _DbContext.AuditApprovals.Add(approval);

            await _DbContext.SaveChangesAsync();
            return true;
        }

        public Task SaveChangesAsync() => _DbContext.SaveChangesAsync();
    }
}

