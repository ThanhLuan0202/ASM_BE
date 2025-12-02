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

        public async Task<List<Finding>> GetFindingsAsync(Guid auditId)
        {
            return await _DbContext.Findings
                .Include(f => f.RootCause)
                .Include(f => f.CreatedByNavigation)  
                .Include(f => f.Reviewer)            
                .Include(f => f.AuditItem)           
                .Include(f => f.Dept)                 
                .Where(f => f.AuditId == auditId)
                .ToListAsync();
        }

        public async Task<List<ViewFindingByMonthCount>> GetFindingsByMonthAsync(Guid auditId)
        {
            var data = await _DbContext.Findings
                .Where(f => f.AuditId == auditId)
                .GroupBy(f => new { f.CreatedAt.Year, f.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync(); // ← EF sẽ chạy SQL tới đây, OK

            // Phần này chạy trên memory (client side)
            var result = data
                .AsEnumerable()
                .Select(g => new ViewFindingByMonthCount
                {
                    Date = new DateTime(g.Year, g.Month, 1),
                    Count = g.Count
                })
                .OrderBy(x => x.Date)
                .ToList();

            return result;
        }

        public async Task<List<(string Department, int Count)>> GetDepartmentFindingsInAuditAsync(Guid auditId)
        {
           
            var result = await _DbContext.Findings
                .Where(f => f.AuditId == auditId)
                .Include(f => f.Dept) 
                .GroupBy(f => f.Dept.Name)
                .Select(g => new
                {
                    Department = g.Key ?? "Unknown",
                    Count = g.Count()
                })
                .ToListAsync();

            return result
                .Select(r => (r.Department, r.Count))
                .ToList();
        }

        public async Task UpdateFindingStatusAsync(Guid findingId, string status)
        {
            var entity = await _context.Findings
                .FirstOrDefaultAsync(x => x.FindingId == findingId);

            if (entity == null)
                throw new Exception("Finding not found");

            entity.Status = status;
            _context.Findings.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ViewFinding>> GetByDepartmentIdAsync(int departmentId)
        {
            if (departmentId <= 0)
                throw new ArgumentException("DepartmentId must be greater than zero.");

            var department = await _DbContext.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptId == departmentId);

            if (department == null)
                throw new InvalidOperationException($"Department with ID '{departmentId}' was not found.");

            var findings = await _DbContext.Findings
                .Where(f => f.DeptId == departmentId)
                .Include(f => f.Audit)
                .Include(f => f.Dept)
                .Include(f => f.CreatedByNavigation)
                .Include(f => f.Reviewer)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewFinding>>(findings);
        }


        public async Task<Guid?> GetAuditIdByFindingIdAsync(Guid findingId)
        {
            var auditId = await _context.Findings
                .Where(f => f.FindingId == findingId)
                .Select(f => f.AuditId)
                .FirstOrDefaultAsync();

            return auditId == Guid.Empty ? null : auditId;
        }

        public async Task<IEnumerable<ViewFinding>> GetByAuditItemIdAsync(Guid auditItemId)
        {
            if (auditItemId == Guid.Empty)
                throw new ArgumentException("AuditItemId cannot be empty");

            var findings = await _DbContext.Findings
                .Where(f => f.AuditItemId == auditItemId)
                .Include(f => f.Audit)
                .Include(f => f.Dept)
                .Include(f => f.CreatedByNavigation)
                .Include(f => f.Reviewer)
                .Include(f => f.AuditItem)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewFinding>>(findings);

        }

        public async Task<Guid?> GetCreatedByIdByFindingIdAsync(Guid findingId)
        {
            return await _DbContext.Findings
                .Where(f => f.FindingId == findingId)
                .Select(f => f.CreatedBy)
                .FirstOrDefaultAsync();
        }

        public async Task<ViewFinding?> SetReceivedAsync(Guid findingId)
        {
            if (findingId == Guid.Empty)
                throw new ArgumentException("FindingId cannot be empty");

            var existing = await _DbContext.Findings
                .AsTracking()
                .FirstOrDefaultAsync(x => x.FindingId == findingId);

            if (existing == null)
                return null;

            // Validate status exists
            var statusExists = await _DbContext.FindingStatuses.AnyAsync(s => s.FindingStatus1 == "Received");
            if (!statusExists)
                throw new InvalidOperationException("Status 'Received' does not exist in FindingStatus table");

            existing.Status = "Received";
            await _DbContext.SaveChangesAsync();

            var updated = await _DbContext.Findings
                .Include(f => f.Audit)
                .Include(f => f.Dept)
                .Include(f => f.CreatedByNavigation)
                .Include(f => f.Reviewer)
                .FirstOrDefaultAsync(f => f.FindingId == findingId);

            return _mapper.Map<ViewFinding>(updated);
        }

        public async Task<IEnumerable<ViewFinding>> GetFindingsByAuditIdAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty");

            var findings = await _DbContext.Findings
                .Where(f => f.AuditId == auditId)
                .Include(f => f.Audit)
                .Include(f => f.Dept)
                .Include(f => f.CreatedByNavigation)
                .Include(f => f.Reviewer)
                .Include(f => f.AuditItem)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewFinding>>(findings);
        }

        public async Task<IEnumerable<ViewFinding>> GetFindingsByCreatedByAsync(Guid createdBy)
        {
            if (createdBy == Guid.Empty)
                throw new ArgumentException("CreatedBy cannot be empty");

            var findings = await _DbContext.Findings
                .Where(f => f.CreatedBy == createdBy)
                .Include(f => f.Audit)
                .Include(f => f.Dept)
                .Include(f => f.CreatedByNavigation)
                .Include(f => f.Reviewer)
                .Include(f => f.AuditItem)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewFinding>>(findings);
        }

        

    }
}
