using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditAssignmentDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditAssignmentRepository : Repository<AuditAssignment>, IAuditAssignmentRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditAssignmentRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
            : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditAssignment>> GetAllAsync()
        {
            var assignments = await _context.AuditAssignments
                .Include(x => x.Audit)
                .Include(x => x.Dept)
                .Include(x => x.Auditor)
                .Where(x => x.Status != "Inactive")
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync();

            return assignments.Select(a => new ViewAuditAssignment
            {
                AssignmentId = a.AssignmentId,
                AuditId = a.AuditId,
                DeptId = a.DeptId,
                AuditorId = a.AuditorId,
                Notes = a.Notes,
                AssignedAt = a.AssignedAt,
                Status = a.Status,
                AuditTitle = a.Audit?.Title,
                DepartmentName = a.Dept?.Name,
                AuditorName = a.Auditor?.FullName
            });
        }

        public async Task<ViewAuditAssignment?> GetByIdAsync(Guid assignmentId)
        {
            if (assignmentId == Guid.Empty)
                throw new ArgumentException("AssignmentId cannot be empty");

            var assignment = await _context.AuditAssignments
                .Include(x => x.Audit)
                .Include(x => x.Dept)
                .Include(x => x.Auditor)
                .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.Status != "Inactive");

            if (assignment == null)
                return null;

            return new ViewAuditAssignment
            {
                AssignmentId = assignment.AssignmentId,
                AuditId = assignment.AuditId,
                DeptId = assignment.DeptId,
                AuditorId = assignment.AuditorId,
                Notes = assignment.Notes,
                AssignedAt = assignment.AssignedAt,
                Status = assignment.Status,
                AuditTitle = assignment.Audit?.Title,
                DepartmentName = assignment.Dept?.Name,
                AuditorName = assignment.Auditor?.FullName
            };
        }

        public async Task<IEnumerable<ViewAuditAssignment>> GetByAuditIdAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty");

            var assignments = await _context.AuditAssignments
                .Include(x => x.Audit)
                .Include(x => x.Dept)
                .Include(x => x.Auditor)
                .Where(x => x.AuditId == auditId && x.Status != "Inactive")
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync();

            return assignments.Select(a => new ViewAuditAssignment
            {
                AssignmentId = a.AssignmentId,
                AuditId = a.AuditId,
                DeptId = a.DeptId,
                AuditorId = a.AuditorId,
                Notes = a.Notes,
                AssignedAt = a.AssignedAt,
                Status = a.Status,
                AuditTitle = a.Audit?.Title,
                DepartmentName = a.Dept?.Name,
                AuditorName = a.Auditor?.FullName
            });
        }

        public async Task<IEnumerable<ViewAuditAssignment>> GetByAuditorIdAsync(Guid auditorId)
        {
            if (auditorId == Guid.Empty)
                throw new ArgumentException("AuditorId cannot be empty");

            var assignments = await _context.AuditAssignments
                .Include(x => x.Audit)
                .Include(x => x.Dept)
                .Include(x => x.Auditor)
                .Where(x => x.AuditorId == auditorId && x.Status != "Inactive")
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync();

            return assignments.Select(a => new ViewAuditAssignment
            {
                AssignmentId = a.AssignmentId,
                AuditId = a.AuditId,
                DeptId = a.DeptId,
                AuditorId = a.AuditorId,
                Notes = a.Notes,
                AssignedAt = a.AssignedAt,
                Status = a.Status,
                AuditTitle = a.Audit?.Title,
                DepartmentName = a.Dept?.Name,
                AuditorName = a.Auditor?.FullName
            });
        }

        public async Task<IEnumerable<ViewAuditAssignment>> GetByDeptIdAsync(int deptId)
        {
            if (deptId <= 0)
                throw new ArgumentException("DeptId must be greater than 0");

            var assignments = await _context.AuditAssignments
                .Include(x => x.Audit)
                .Include(x => x.Dept)
                .Include(x => x.Auditor)
                .Where(x => x.DeptId == deptId && x.Status != "Inactive")
                .OrderByDescending(x => x.AssignedAt)
                .ToListAsync();

            return assignments.Select(a => new ViewAuditAssignment
            {
                AssignmentId = a.AssignmentId,
                AuditId = a.AuditId,
                DeptId = a.DeptId,
                AuditorId = a.AuditorId,
                Notes = a.Notes,
                AssignedAt = a.AssignedAt,
                Status = a.Status,
                AuditTitle = a.Audit?.Title,
                DepartmentName = a.Dept?.Name,
                AuditorName = a.Auditor?.FullName
            });
        }

        public async Task<ViewAuditAssignment> CreateAsync(CreateAuditAssignment dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            // Validate Audit exists
            var auditExists = await _context.Audits.AnyAsync(a => a.AuditId == dto.AuditId);
            if (!auditExists)
                throw new InvalidOperationException($"Audit with ID {dto.AuditId} does not exist");

            // Validate Department exists
            var deptExists = await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId);
            if (!deptExists)
                throw new InvalidOperationException($"Department with ID {dto.DeptId} does not exist");

            // Validate Auditor exists
            var auditorExists = await _context.UserAccounts.AnyAsync(u => u.UserId == dto.AuditorId);
            if (!auditorExists)
                throw new InvalidOperationException($"User with ID {dto.AuditorId} does not exist");

            var entity = new AuditAssignment
            {
                AssignmentId = Guid.NewGuid(),
                AuditId = dto.AuditId,
                DeptId = dto.DeptId,
                AuditorId = dto.AuditorId,
                Notes = dto.Notes,
                AssignedAt = DateTime.UtcNow,
                Status = dto.Status ?? "Assigned"
            };

            _context.AuditAssignments.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.AuditAssignments
                .Include(x => x.Audit)
                .Include(x => x.Dept)
                .Include(x => x.Auditor)
                .FirstOrDefaultAsync(x => x.AssignmentId == entity.AssignmentId);

            return new ViewAuditAssignment
            {
                AssignmentId = created.AssignmentId,
                AuditId = created.AuditId,
                DeptId = created.DeptId,
                AuditorId = created.AuditorId,
                Notes = created.Notes,
                AssignedAt = created.AssignedAt,
                Status = created.Status,
                AuditTitle = created.Audit?.Title,
                DepartmentName = created.Dept?.Name,
                AuditorName = created.Auditor?.FullName
            };
        }

        public async Task<ViewAuditAssignment?> UpdateAsync(Guid assignmentId, UpdateAuditAssignment dto)
        {
            if (assignmentId == Guid.Empty)
                throw new ArgumentException("AssignmentId cannot be empty");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _context.AuditAssignments
                .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.Status != "Inactive");

            if (entity == null)
                return null;

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(dto.Notes))
                entity.Notes = dto.Notes;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                entity.Status = dto.Status;

            await _context.SaveChangesAsync();

            var updated = await _context.AuditAssignments
                .Include(x => x.Audit)
                .Include(x => x.Dept)
                .Include(x => x.Auditor)
                .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId);

            return new ViewAuditAssignment
            {
                AssignmentId = updated.AssignmentId,
                AuditId = updated.AuditId,
                DeptId = updated.DeptId,
                AuditorId = updated.AuditorId,
                Notes = updated.Notes,
                AssignedAt = updated.AssignedAt,
                Status = updated.Status,
                AuditTitle = updated.Audit?.Title,
                DepartmentName = updated.Dept?.Name,
                AuditorName = updated.Auditor?.FullName
            };
        }

        public async Task<bool> DeleteAsync(Guid assignmentId)
        {
            if (assignmentId == Guid.Empty)
                throw new ArgumentException("AssignmentId cannot be empty");

            var entity = await _context.AuditAssignments
                .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.Status != "Inactive");

            if (entity == null)
                return false;

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(Guid assignmentId)
        {
            if (assignmentId == Guid.Empty)
                return false;

            return await _context.AuditAssignments
                .AnyAsync(x => x.AssignmentId == assignmentId && x.Status != "Inactive");
        }
    }
}

