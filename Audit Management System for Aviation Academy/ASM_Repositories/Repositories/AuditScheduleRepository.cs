using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditScheduleDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditScheduleRepository : Repository<AuditSchedule>, IAuditScheduleRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditScheduleRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
            : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditSchedule>> GetAllAsync()
        {
            var schedules = await _context.AuditSchedules
                .Include(x => x.Audit)
                .Where(x => x.Status != "Inactive")
                .OrderBy(x => x.DueDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditSchedule>>(schedules);
        }

        public async Task<ViewAuditSchedule?> GetByIdAsync(Guid scheduleId)
        {
            if (scheduleId == Guid.Empty)
                throw new ArgumentException("ScheduleId cannot be empty");

            var schedule = await _context.AuditSchedules
                .Include(x => x.Audit)
                .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId && x.Status != "Inactive");

            return schedule == null ? null : _mapper.Map<ViewAuditSchedule>(schedule);
        }

        public async Task<IEnumerable<ViewAuditSchedule>> GetByAuditIdAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty");

            var schedules = await _context.AuditSchedules
                .Include(x => x.Audit)
                .Where(x => x.AuditId == auditId && x.Status != "Inactive")
                .OrderBy(x => x.DueDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditSchedule>>(schedules);
        }

        public async Task<ViewAuditSchedule> CreateAsync(CreateAuditSchedule dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var auditExists = await _context.Audits.AnyAsync(a => a.AuditId == dto.AuditId);
            if (!auditExists)
                throw new InvalidOperationException($"Audit with ID {dto.AuditId} does not exist");

            var duplicateExists = await _context.AuditSchedules
                .AnyAsync(x => x.AuditId == dto.AuditId && 
                              x.MilestoneName == dto.MilestoneName && 
                              x.Status != "Inactive");
            if (duplicateExists)
                throw new InvalidOperationException($"A schedule with MilestoneName '{dto.MilestoneName}' already exists for this Audit");

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                
            }

            var entity = _mapper.Map<AuditSchedule>(dto);
            entity.ScheduleId = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.Status = dto.Status ?? "Active";

            _context.AuditSchedules.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.AuditSchedules
                .Include(x => x.Audit)
                .FirstOrDefaultAsync(x => x.ScheduleId == entity.ScheduleId);

            return _mapper.Map<ViewAuditSchedule>(created);
        }

        public async Task<ViewAuditSchedule?> UpdateAsync(Guid scheduleId, UpdateAuditSchedule dto)
        {
            if (scheduleId == Guid.Empty)
                throw new ArgumentException("ScheduleId cannot be empty");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _context.AuditSchedules
                .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId && x.Status != "Inactive");

            if (entity == null)
                return null;

            var duplicateExists = await _context.AuditSchedules
                .AnyAsync(x => x.AuditId == entity.AuditId && 
                              x.MilestoneName == dto.MilestoneName && 
                              x.ScheduleId != scheduleId &&
                              x.Status != "Inactive");
            if (duplicateExists)
                throw new InvalidOperationException($"A schedule with MilestoneName '{dto.MilestoneName}' already exists for this Audit");

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
            }

            entity.MilestoneName = dto.MilestoneName;
            entity.DueDate = dto.DueDate;
            entity.Notes = dto.Notes;
            entity.Status = dto.Status ?? entity.Status;

            await _context.SaveChangesAsync();

            var updated = await _context.AuditSchedules
                .Include(x => x.Audit)
                .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId);

            return _mapper.Map<ViewAuditSchedule>(updated);
        }

        public async Task<bool> DeleteAsync(Guid scheduleId)
        {
            if (scheduleId == Guid.Empty)
                throw new ArgumentException("ScheduleId cannot be empty");

            var entity = await _context.AuditSchedules
                .FirstOrDefaultAsync(x => x.ScheduleId == scheduleId && x.Status != "Inactive");

            if (entity == null)
                return false;

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(Guid scheduleId)
        {
            if (scheduleId == Guid.Empty)
                return false;

            return await _context.AuditSchedules
                .AnyAsync(x => x.ScheduleId == scheduleId && x.Status != "Inactive");
        }

        public async Task UpdateSchedulesAsync(Guid auditId, List<UpdateAuditSchedule>? list)
        {
            if (list == null || !list.Any())
                return; // Không có gì để update, bỏ qua

            // Xóa schedule cũ
            var existing = _context.AuditSchedules
                .Where(x => x.AuditId == auditId);
            _context.AuditSchedules.RemoveRange(existing);

            // Thêm schedule mới
            foreach (var item in list)
            {
                var entity = _mapper.Map<AuditSchedule>(item);
                entity.AuditId = auditId;
                await _context.AuditSchedules.AddAsync(entity);
            }
        }

        public async Task UpdateStatusToArchivedAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty.");

            var entities = await _context.AuditSchedules
                .Where(a => a.AuditId == auditId)
                .ToListAsync();

            if (!entities.Any())
                throw new InvalidOperationException($"No AuditSchedule found for AuditId '{auditId}'.");

            foreach (var entity in entities)
            {
                entity.Status = "Archived";
                _context.Entry(entity).Property(x => x.Status).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }


    }
}

