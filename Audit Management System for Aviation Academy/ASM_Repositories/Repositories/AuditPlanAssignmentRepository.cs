using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditPlanAssignmentRepository : IAuditPlanAssignmentRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditPlanAssignmentRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditPlanAssignment>> GetAllAsync()
        {
            var list = await _context.AuditPlanAssignments
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditPlanAssignment>>(list);
        }

        public async Task<ViewAuditPlanAssignment?> GetByIdAsync(Guid id)
        {
            var entity = await _context.AuditPlanAssignments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AssignmentId == id && a.Status != "Inactive");

            return entity == null ? null : _mapper.Map<ViewAuditPlanAssignment>(entity);
        }

        public async Task<ViewAuditPlanAssignment> CreateAsync(CreateAuditPlanAssignment dto)
        {
            try
            {
                var entity = _mapper.Map<AuditPlanAssignment>(dto);
                entity.AssignmentId = Guid.NewGuid();
                entity.Status = "Active";

                _context.AuditPlanAssignments.Add(entity);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewAuditPlanAssignment>(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AuditPlanAssignmentRepository.CreateAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while creating the audit plan assignment.", ex);
            }
        }

        public async Task<ViewAuditPlanAssignment?> UpdateAsync(Guid id, UpdateAuditPlanAssignment dto)
        {
            try
            {
                var existing = await _context.AuditPlanAssignments
                    .FirstOrDefaultAsync(a => a.AssignmentId == id && a.Status != "Inactive");

                if (existing == null)
                    return null;

                _mapper.Map(dto, existing);
                _context.AuditPlanAssignments.Update(existing);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewAuditPlanAssignment>(existing);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AuditPlanAssignmentRepository.UpdateAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while updating the audit plan assignment.", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.AuditPlanAssignments
                .FirstOrDefaultAsync(a => a.AssignmentId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            entity.Status = "Inactive";
            _context.AuditPlanAssignments.Update(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ViewAuditPlanAssignment>> GetAssignmentsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var assignments = await _context.AuditPlanAssignments
                .Include(apa => apa.Auditor)
                .Include(apa => apa.AssignByNavigation)
                .Where(apa => apa.Status == "Active" 
                    && apa.AssignedDate >= startDate 
                    && apa.AssignedDate <= endDate)
                .OrderBy(apa => apa.AssignedDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditPlanAssignment>>(assignments);
        }

        public async Task<int> GetAssignmentCountByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            // Đếm số lượng auditors đã được assign và đã tạo audits trong thời kỳ này
            var count = await _context.AuditPlanAssignments
                .Where(apa => apa.Status == "Active")
                .Join(
                    _context.Audits.Where(a => a.StartDate >= startDate && a.EndDate <= endDate && a.Status != "Inactive"),
                    apa => apa.AuditorId,
                    a => a.CreatedBy,
                    (apa, a) => apa.AuditorId
                )
                .Distinct()
                .CountAsync();

            return count;
        }

        public async Task<bool> HasActiveAssignmentByAuditorIdAsync(Guid auditorId)
        {
            var exists = await _context.AuditPlanAssignments
                .AnyAsync(apa => apa.AuditorId == auditorId && apa.Status == "Active");
            
            return exists;
        }

        public async Task UpdateStatusToArchivedAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty.");

            // AuditPlanAssignment doesn't have AuditId directly, so we update assignments
            // for auditors who created the audit (CreatedBy = AuditorId)
            var audit = await _context.Audits
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AuditId == auditId);

            if (audit == null)
                return;

            // Update all active assignments for the auditor who created this audit
            var entities = await _context.AuditPlanAssignments
                .Where(apa => apa.AuditorId == audit.CreatedBy && apa.Status == "Active")
                .ToListAsync();

            if (!entities.Any())
                return;

            foreach (var entity in entities)
            {
                entity.Status = "Archived";
                _context.Entry(entity).Property(x => x.Status).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
