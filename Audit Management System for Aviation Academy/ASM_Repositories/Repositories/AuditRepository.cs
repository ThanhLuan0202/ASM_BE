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
                // Validate trước khi xóa và thêm mới
                // 1. Kiểm tra UserId có trong mỗi item không
                var itemsWithoutUserId = dto.AuditTeams.Where(t => !t.UserId.HasValue || t.UserId.Value == Guid.Empty).ToList();
                if (itemsWithoutUserId.Any())
                    throw new InvalidOperationException("UserId is required for all AuditTeam items in complete-update");

                // 2. Kiểm tra duplicate UserId trong cùng một request
                var userIds = dto.AuditTeams.Select(t => t.UserId.Value).ToList();
                var duplicateUserIds = userIds.GroupBy(u => u).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (duplicateUserIds.Any())
                    throw new InvalidOperationException($"Duplicate UserId found in AuditTeams: {string.Join(", ", duplicateUserIds)}. Each user can only be assigned once per audit.");

                // 3. Kiểm tra chỉ có 1 Lead Auditor
                /*
                var leadCount = dto.AuditTeams.Count(t => t.IsLead.HasValue && t.IsLead.Value);
                if (leadCount > 1)
                    throw new InvalidOperationException("Only one Lead Auditor (IsLead = true) is allowed per audit");
                if (leadCount == 0)
                    throw new InvalidOperationException("At least one Lead Auditor (IsLead = true) is required");
                */
                // 4. Validate tất cả UserId tồn tại
                foreach (var item in dto.AuditTeams)
                {
                    var userExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == item.UserId.Value);
                    if (!userExists)
                        throw new InvalidOperationException($"User with ID {item.UserId} does not exist");
                }

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
                    entity.UserId = item.UserId.Value; // Đảm bảo UserId được set
                    if (string.IsNullOrEmpty(entity.Status))
                        entity.Status = "Active";
                    
                    if (item.IsLead.HasValue)
                        entity.IsLead = item.IsLead.Value;
                    else
                        entity.IsLead = false;
                    await _DbContext.AuditTeams.AddAsync(entity);
                }
            }

            // Update Schedules nếu có
            if (dto.Schedules != null && dto.Schedules.Any())
            {
                // Validate trước khi xóa và thêm mới
                // 1. Kiểm tra MilestoneName và DueDate có trong mỗi item không
                foreach (var item in dto.Schedules)
                {
                    if (string.IsNullOrWhiteSpace(item.MilestoneName))
                        throw new InvalidOperationException("MilestoneName is required for all AuditSchedule items");
                    
                    if (item.DueDate == default(DateTime))
                        throw new InvalidOperationException("DueDate is required for all AuditSchedule items");
                }

                // 2. Kiểm tra duplicate MilestoneName trong cùng một request
                var milestoneNames = dto.Schedules.Select(s => s.MilestoneName.Trim()).ToList();
                var duplicateMilestones = milestoneNames.GroupBy(m => m, StringComparer.OrdinalIgnoreCase)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                if (duplicateMilestones.Any())
                    throw new InvalidOperationException($"Duplicate MilestoneName found in Schedules: {string.Join(", ", duplicateMilestones)}. Each milestone name must be unique per audit.");

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


            // Update AuditChecklistTemplateMaps nếu có
            if (dto.AuditChecklistTemplateMaps != null && dto.AuditChecklistTemplateMaps.Any())
            {
                // Xóa tất cả mapping cũ của audit này
                var existingMaps = _DbContext.AuditChecklistTemplateMaps
                    .Where(x => x.AuditId == auditId);
                _DbContext.AuditChecklistTemplateMaps.RemoveRange(existingMaps);

                // Thêm mapping mới
                foreach (var mapDto in dto.AuditChecklistTemplateMaps)
                {
                    // Validate AuditId phải khớp với auditId trong URL (hoặc không cần gửi, sẽ tự set)
                    // Nếu có gửi AuditId thì phải khớp
                    if (mapDto.AuditId != Guid.Empty && mapDto.AuditId != auditId)
                        throw new InvalidOperationException($"AuditId in map ({mapDto.AuditId}) does not match auditId in URL ({auditId})");

                    // Validate TemplateId bắt buộc phải có
                    if (mapDto.TemplateId == Guid.Empty)
                        throw new InvalidOperationException("TemplateId is required in AuditChecklistTemplateMap");

                    // Validate TemplateId tồn tại
                    var templateExists = await _DbContext.ChecklistTemplates.AnyAsync(t => t.TemplateId == mapDto.TemplateId);
                    if (!templateExists)
                        throw new InvalidOperationException($"ChecklistTemplate with ID {mapDto.TemplateId} does not exist");

                    // Validate AssignedBy nếu có
                    if (mapDto.AssignedBy.HasValue)
                    {
                        var userExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == mapDto.AssignedBy.Value);
                        if (!userExists)
                            throw new InvalidOperationException($"User with ID {mapDto.AssignedBy} does not exist");
                    }

                    // Kiểm tra duplicate TemplateId trong cùng một request
                    var duplicateCount = dto.AuditChecklistTemplateMaps.Count(m => m.TemplateId == mapDto.TemplateId);
                    if (duplicateCount > 1)
                        throw new InvalidOperationException($"Duplicate TemplateId {mapDto.TemplateId} found in AuditChecklistTemplateMaps. Each template can only be mapped once per audit.");

                    var entity = _mapper.Map<AuditChecklistTemplateMap>(mapDto);
                    entity.AuditId = auditId; // Đảm bảo AuditId đúng từ URL
                    entity.AssignedAt = DateTime.UtcNow;
                    if (string.IsNullOrEmpty(entity.Status))
                        entity.Status = "Active";
                    await _DbContext.AuditChecklistTemplateMaps.AddAsync(entity);
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

        public async Task<IEnumerable<ViewAudit>> GetAuditsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var audits = await _DbContext.Audits
                .Include(a => a.CreatedByNavigation)
                .Include(a => a.Template)
                .Include(a => a.StatusNavigation)
                .Where(a => a.StartDate >= startDate 
                    && a.EndDate <= endDate 
                    && a.Status != "Inactive")
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAudit>>(audits);
        }

        public async Task<List<int>> GetUsedDepartmentsByPeriodAsync(DateTime startDate, DateTime endDate, Guid? excludeAuditId = null)
        {
            var query = _DbContext.AuditScopeDepartments
                .Include(asd => asd.Audit)
                .Where(asd => asd.Audit.StartDate >= startDate 
                    && asd.Audit.EndDate <= endDate
                    && asd.Status == "Active"
                    && asd.Audit.Status != "Inactive");

            if (excludeAuditId.HasValue)
            {
                query = query.Where(asd => asd.AuditId != excludeAuditId.Value);
            }

            var departmentIds = await query
                .Select(asd => asd.DeptId)
                .Distinct()
                .ToListAsync();

            return departmentIds;
        }

    }
}

