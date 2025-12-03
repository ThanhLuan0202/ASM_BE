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
            _DbContext.Entry(existing).State = EntityState.Modified;
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
            var existing = await _DbContext.Audits.AsTracking().FirstOrDefaultAsync(x => x.AuditId == id);
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
            var audit = await _DbContext.Audits
                .AsTracking()
                .FirstOrDefaultAsync(a => a.AuditId == auditId);
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
            var audit = await _DbContext.Audits
                .AsTracking()
                .FirstOrDefaultAsync(a => a.AuditId == auditId);
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
                ApprovalLevel = "Director",
                Status = "Rejected Plan",
                Comment = comment,
                CreatedAt = DateTime.UtcNow,
                ApprovedAt = DateTime.UtcNow
            };
            _DbContext.AuditApprovals.Add(approval);

            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeclinedPlanContentAsync(Guid auditId, Guid approverId, string comment)
        {
            var audit = await _DbContext.Audits
                .AsTracking()
                .FirstOrDefaultAsync(a => a.AuditId == auditId);
            if (audit == null)
            {
                return false;
            }

            var approverExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == approverId);
            if (!approverExists)
            {
                throw new InvalidOperationException($"ApproverId '{approverId}' does not exist");
            }

            var rejectedStatusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == "Declined");
            if (!rejectedStatusExists)
            {
                throw new InvalidOperationException("Status 'Declined' does not exist in AuditStatus");
            }

            audit.Status = "Declined";

            var approval = new AuditApproval
            {
                AuditApprovalId = Guid.NewGuid(),
                AuditId = auditId,
                ApproverId = approverId,
                ApprovalLevel = "Lead Auditor",
                Status = "Declined Plan",
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
            var audit = await _DbContext.Audits
                .AsTracking()
                .FirstOrDefaultAsync(a => a.AuditId == auditId);
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
            var audit = await _DbContext.Audits
                .AsTracking()
                .FirstOrDefaultAsync(a => a.AuditId == auditId);
            if (audit == null)
            {
                return false;
            }

            var approverExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == approverId);
            if (!approverExists)
            {
                throw new InvalidOperationException($"ApproverId '{approverId}' does not exist");
            }

            var approveStatusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == "InProgress");
            if (!approveStatusExists)
            {
                throw new InvalidOperationException("Status 'InProgress' does not exist in AuditStatus");
            }

            audit.Status = "InProgress";
            audit.IsPublished = true;
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

        public async Task<UserAccount?> GetLeadAuditorAsync(Guid auditId)
        {
            var lead = await _DbContext.AuditTeams
                .Include(at => at.User)
                .Where(at => at.AuditId == auditId
                    && at.Status == "Active"
                    && (at.RoleInTeam == "LeadAuditor"))
                .Select(at => new UserAccount
                {
                    UserId = at.UserId,
                    FullName = at.User.FullName,
                    Email = at.User.Email
                })
                .FirstOrDefaultAsync();

            return lead;
        }


        public Task SaveChangesAsync() => _DbContext.SaveChangesAsync();

        public async Task<Audit?> UpdateStatusByAuditIdAsync(Guid auditId, string status)
        {
            var audit = await _DbContext.Audits
                .FirstOrDefaultAsync(a => a.AuditId == auditId);

            if (audit != null)
            {
                _DbContext.Audits.Attach(audit);
                audit.Status = status;

                await _DbContext.SaveChangesAsync();
            }

            return audit;
        }

        public async Task UpdateAuditPlanAsync(Audit audit, UpdateAudit? updateAudit)
        {
            if (updateAudit == null)
                return; // Không có gì để update

            // Map chỉ các property có giá trị
            _mapper.Map(updateAudit, audit);

            // Chỉ update audit, EF tự theo dõi entity
            _DbContext.Audits.Update(audit);
        }


        public async Task<Audit?> GetAuditPlanAsync(Guid auditId)
        {
            return await _context.Audits
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.AuditScopeDepartments)
                    .ThenInclude(x => x.Dept)
                .Include(a => a.AuditCriteriaMaps)
                    .ThenInclude(x => x.Criteria)
                .Include(a => a.AuditTeams)
                    .ThenInclude(x => x.User)
                .Include(a => a.AuditSchedules)
                .FirstOrDefaultAsync(a => a.AuditId == auditId);
        }

        public async Task UpdateStatusToArchivedAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty.");

            var audit = await _context.Audits.FirstOrDefaultAsync(a => a.AuditId == auditId);
            if (audit == null)
                throw new InvalidOperationException($"Audit with ID '{auditId}' does not exist.");

            var statusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == "Archived");
            if (!statusExists)
            {
                throw new InvalidOperationException($"Status 'Archived' does not exist");
            }

            audit.Status = "Archived";
            _DbContext.Entry(audit).Property(x => x.Status).IsModified = true;
            await _DbContext.SaveChangesAsync();
        }

        public async Task<ViewAudit?> UpdateAuditCompleteAsync(Guid auditId, UpdateAuditComplete dto)
        {
            // Kiểm tra audit có tồn tại không
            var audit = await _DbContext.Audits
                .AsTracking()
                .FirstOrDefaultAsync(a => a.AuditId == auditId);

            if (audit == null)
            {
                return null;
            }

            // Update audit nếu có
            if (dto.Audit != null)
            {
                // Validate TemplateId nếu có
                if (dto.Audit.TemplateId.HasValue)
                {
                    var templateExists = await _DbContext.ChecklistTemplates.AnyAsync(t => t.TemplateId == dto.Audit.TemplateId.Value);
                    if (!templateExists)
                    {
                        throw new InvalidOperationException($"Template with ID {dto.Audit.TemplateId} does not exist");
                    }
                }

                // Validate Status nếu có
                if (!string.IsNullOrEmpty(dto.Audit.Status))
                {
                    var statusExists = await _DbContext.AuditStatuses.AnyAsync(s => s.AuditStatus1 == dto.Audit.Status);
                    if (!statusExists)
                    {
                        throw new InvalidOperationException($"Status '{dto.Audit.Status}' does not exist");
                    }
                }

                // Validate StartDate và EndDate
                if (dto.Audit.StartDate.HasValue && dto.Audit.EndDate.HasValue && dto.Audit.StartDate.Value > dto.Audit.EndDate.Value)
                {
                    throw new InvalidOperationException("StartDate cannot be later than EndDate");
                }

                // Map và update audit
                _mapper.Map(dto.Audit, audit);
                _DbContext.Audits.Update(audit);
            }

            // Update CriteriaMaps nếu có
            if (dto.CriteriaMaps != null && dto.CriteriaMaps.Any())
            {
                // Xóa criteria cũ
                var existingCriteria = _DbContext.AuditCriteriaMaps
                    .Where(x => x.AuditId == auditId);
                _DbContext.AuditCriteriaMaps.RemoveRange(existingCriteria);

                // Thêm criteria mới
                foreach (var item in dto.CriteriaMaps)
                {
                    var entity = _mapper.Map<AuditCriteriaMap>(item);
                    entity.AuditId = auditId;
                    if (string.IsNullOrEmpty(entity.Status))
                        entity.Status = "Active";
                    await _DbContext.AuditCriteriaMaps.AddAsync(entity);
                }
            }

            // Update ScopeDepartments nếu có
            if (dto.ScopeDepartments != null && dto.ScopeDepartments.Any())
            {
                // Xóa scope cũ
                var existingScope = _DbContext.AuditScopeDepartments
                    .Where(x => x.AuditId == auditId);
                _DbContext.AuditScopeDepartments.RemoveRange(existingScope);

                // Thêm scope mới
                foreach (var item in dto.ScopeDepartments)
                {
                    var entity = _mapper.Map<AuditScopeDepartment>(item);
                    entity.AuditScopeId = Guid.NewGuid();
                    entity.AuditId = auditId;
                    if (string.IsNullOrEmpty(entity.Status))
                        entity.Status = "Active";
                    await _DbContext.AuditScopeDepartments.AddAsync(entity);
                }
            }

            // Update AuditTeams nếu có
            if (dto.AuditTeams != null && dto.AuditTeams.Any())
            {
                // Xóa team cũ
                var existingTeams = _DbContext.AuditTeams
                    .Where(x => x.AuditId == auditId);
                _DbContext.AuditTeams.RemoveRange(existingTeams);

                // Thêm team mới
                foreach (var item in dto.AuditTeams)
                {
                    var entity = _mapper.Map<AuditTeam>(item);
                    entity.AuditTeamId = Guid.NewGuid();
                    entity.AuditId = auditId;
                    if (string.IsNullOrEmpty(entity.Status))
                        entity.Status = "Active";
                    await _DbContext.AuditTeams.AddAsync(entity);
                }
            }

            // Update Schedules nếu có
            if (dto.Schedules != null && dto.Schedules.Any())
            {
                // Xóa schedule cũ
                var existingSchedules = _DbContext.AuditSchedules
                    .Where(x => x.AuditId == auditId);
                _DbContext.AuditSchedules.RemoveRange(existingSchedules);

                // Thêm schedule mới
                foreach (var item in dto.Schedules)
                {
                    var entity = _mapper.Map<AuditSchedule>(item);
                    entity.ScheduleId = Guid.NewGuid();
                    entity.AuditId = auditId;
                    entity.CreatedAt = DateTime.UtcNow;
                    if (string.IsNullOrEmpty(entity.Status))
                        entity.Status = "Active";
                    await _DbContext.AuditSchedules.AddAsync(entity);
                }
            }

            // Update ChecklistItems nếu có
            if (dto.ChecklistItems != null && dto.ChecklistItems.Any())
            {
                // Xóa checklist items cũ
                var existingChecklistItems = _DbContext.AuditChecklistItems
                    .Where(x => x.AuditId == auditId);
                _DbContext.AuditChecklistItems.RemoveRange(existingChecklistItems);

                // Thêm checklist items mới
                foreach (var item in dto.ChecklistItems)
                {
                    var entity = _mapper.Map<AuditChecklistItem>(item);
                    entity.AuditItemId = Guid.NewGuid();
                    entity.AuditId = auditId;
                    if (string.IsNullOrEmpty(entity.Status))
                        entity.Status = "Active";
                    await _DbContext.AuditChecklistItems.AddAsync(entity);
                }
            }

            // KHÔNG save ở đây - để Service layer quản lý transaction
            // await _DbContext.SaveChangesAsync();

            // Lấy lại audit đã update với các relations
            var updatedAudit = await _DbContext.Audits
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.Template)
                .Include(a => a.StatusNavigation)
                .FirstOrDefaultAsync(a => a.AuditId == auditId);

            return updatedAudit == null ? null : _mapper.Map<ViewAudit>(updatedAudit);
        }

    }
}

