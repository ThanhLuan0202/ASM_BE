using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class FindingRepository : Repository<Finding>, IFindingRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IMapper _mapper;

        public FindingRepository(AuditManagementSystemForAviationAcademyContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewFinding>> GetAllFindingAsync()
        {
            var findings = await _DbContext.Findings
                .Include(f => f.Audit)
                .Include(f => f.Dept)
                .Include(f => f.CreatedByNavigation)
                .Include(f => f.Reviewer)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewFinding>>(findings);
        }

        public async Task<ViewFinding?> GetFindingByIdAsync(Guid id)
        {
            var finding = await _DbContext.Findings
                .Include(f => f.Audit)
                .Include(f => f.Dept)
                .Include(f => f.CreatedByNavigation)
                .Include(f => f.Reviewer)
                .FirstOrDefaultAsync(f => f.FindingId == id);

            return finding == null ? null : _mapper.Map<ViewFinding>(finding);
        }

        public async Task<ViewFinding> CreateFindingAsync(CreateFinding dto, Guid? createdByUserId)
        {
            var auditExists = await _DbContext.Audits.AnyAsync(a => a.AuditId == dto.AuditId);
            if (!auditExists)
            {
                throw new InvalidOperationException($"Audit with ID {dto.AuditId} does not exist");
            }

            if (dto.AuditItemId.HasValue)
            {
                var auditItemExists = await _DbContext.AuditChecklistItems
                    .AnyAsync(a => a.AuditItemId == dto.AuditItemId.Value && a.AuditId == dto.AuditId);
                if (!auditItemExists)
                {
                    throw new InvalidOperationException($"AuditItem with ID {dto.AuditItemId} does not exist for Audit {dto.AuditId}");
                }
            }

            if (dto.DeptId.HasValue)
            {
                var deptExists = await _DbContext.Departments.AnyAsync(d => d.DeptId == dto.DeptId.Value);
                if (!deptExists)
                {
                    throw new InvalidOperationException($"Department with ID {dto.DeptId} does not exist");
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

            if (dto.ReviewerId.HasValue)
            {
                var reviewerExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == dto.ReviewerId.Value);
                if (!reviewerExists)
                {
                    throw new InvalidOperationException($"Reviewer with ID {dto.ReviewerId} does not exist");
                }
            }

            if (!string.IsNullOrEmpty(dto.Severity))
            {
                var severityExists = await _DbContext.FindingSeverities.AnyAsync(s => s.Severity == dto.Severity);
                if (!severityExists)
                {
                    throw new InvalidOperationException($"Severity '{dto.Severity}' does not exist");
                }
            }

            string status = dto.Status ?? "Open";
            if (!string.IsNullOrEmpty(status))
            {
                var statusExists = await _DbContext.FindingStatuses.AnyAsync(s => s.FindingStatus1 == status);
                if (!statusExists)
                {
                    throw new InvalidOperationException($"Status '{status}' does not exist");
                }
            }

            if (dto.RootCauseId.HasValue)
            {
                var rootCauseExists = await _DbContext.RootCauses.AnyAsync(r => r.RootCauseId == dto.RootCauseId.Value);
                if (!rootCauseExists)
                {
                    throw new InvalidOperationException($"RootCause with ID {dto.RootCauseId} does not exist");
                }
            }

            var finding = _mapper.Map<Finding>(dto);
            finding.FindingId = Guid.NewGuid();
            finding.CreatedAt = DateTime.UtcNow;
            finding.Status = status;
            finding.CreatedBy = createdByUserId; 

            _DbContext.Findings.Add(finding);
            await _DbContext.SaveChangesAsync();

            var createdFinding = await _DbContext.Findings
                .Include(f => f.Audit)
                .Include(f => f.Dept)
                .Include(f => f.CreatedByNavigation)
                .Include(f => f.Reviewer)
                .FirstOrDefaultAsync(f => f.FindingId == finding.FindingId);

            return _mapper.Map<ViewFinding>(createdFinding);
        }

        public async Task<ViewFinding?> UpdateFindingAsync(Guid id, UpdateFinding dto)
        {
            var existing = await _DbContext.Findings.FindAsync(id);
            if (existing == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Severity))
            {
                var severityExists = await _DbContext.FindingSeverities.AnyAsync(s => s.Severity == dto.Severity);
                if (!severityExists)
                {
                    throw new InvalidOperationException($"Severity '{dto.Severity}' does not exist");
                }
            }

            if (!string.IsNullOrEmpty(dto.Status))
            {
                var statusExists = await _DbContext.FindingStatuses.AnyAsync(s => s.FindingStatus1 == dto.Status);
                if (!statusExists)
                {
                    throw new InvalidOperationException($"Status '{dto.Status}' does not exist");
                }
            }

            if (dto.DeptId.HasValue)
            {
                var deptExists = await _DbContext.Departments.AnyAsync(d => d.DeptId == dto.DeptId.Value);
                if (!deptExists)
                {
                    throw new InvalidOperationException($"Department with ID {dto.DeptId} does not exist");
                }
            }

            if (dto.ReviewerId.HasValue)
            {
                var reviewerExists = await _DbContext.UserAccounts.AnyAsync(u => u.UserId == dto.ReviewerId.Value);
                if (!reviewerExists)
                {
                    throw new InvalidOperationException($"Reviewer with ID {dto.ReviewerId} does not exist");
                }
            }

            if (dto.RootCauseId.HasValue)
            {
                var rootCauseExists = await _DbContext.RootCauses.AnyAsync(r => r.RootCauseId == dto.RootCauseId.Value);
                if (!rootCauseExists)
                {
                    throw new InvalidOperationException($"RootCause with ID {dto.RootCauseId} does not exist");
                }
            }

            if (dto.AuditItemId.HasValue)
            {
                var auditItemExists = await _DbContext.AuditChecklistItems
                    .AnyAsync(a => a.AuditItemId == dto.AuditItemId.Value && a.AuditId == existing.AuditId);
                if (!auditItemExists)
                {
                    throw new InvalidOperationException($"AuditItem with ID {dto.AuditItemId} does not exist for Audit {existing.AuditId}");
                }
            }

            _mapper.Map(dto, existing);
            await _DbContext.SaveChangesAsync();

            var updatedFinding = await _DbContext.Findings
                .Include(f => f.Audit)
                .Include(f => f.Dept)
                .Include(f => f.CreatedByNavigation)
                .Include(f => f.Reviewer)
                .FirstOrDefaultAsync(f => f.FindingId == id);

            return _mapper.Map<ViewFinding>(updatedFinding);
        }

        public async Task<bool> DeleteFindingAsync(Guid id)
        {
            var existing = await _DbContext.Findings.FindAsync(id);
            if (existing == null)
            {
                return false;
            }

            var inactiveStatusExists = await _DbContext.FindingStatuses.AnyAsync(s => s.FindingStatus1 == "Inactive");
            if (!inactiveStatusExists)
            {
                throw new InvalidOperationException("Status 'Inactive' does not exist in the system. Please add it to FindingStatus table first.");
            }

            existing.Status = "Inactive";
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _DbContext.Findings.AnyAsync(f => f.FindingId == id);
        }
    }
}
