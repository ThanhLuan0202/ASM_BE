using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Helper;
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

        public async Task<List<Guid>> MarkEvidenceDueOverdueAsync(CancellationToken ct = default)
        {
            // Lấy danh sách schedules
            var schedules = await _context.AuditSchedules.AsNoTracking()
                .Where(x =>
                    x.MilestoneName == "Evidence Due" &&
                    x.Status == "Active")
                .ToListAsync(ct);

            if (!schedules.Any())
                return new List<Guid>();

            // So sánh với thời gian hiện tại theo local timezone
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneHelper.TimeZone);
            var todayLocal = nowLocal.Date;

            var overdueAuditIds = schedules
                .Where(x =>
                {
                    // Convert DueDate từ DB sang local time để so sánh
                    DateTime dueDateLocal;
                    if (x.DueDate.Kind == DateTimeKind.Utc)
                    {
                        dueDateLocal = TimeZoneInfo.ConvertTimeFromUtc(x.DueDate, TimeZoneHelper.TimeZone);
                    }
                    else
                    {
                        // Unspecified - giả định là local time (date only)
                        dueDateLocal = x.DueDate;
                    }
                    
                    // Overdue nếu: DueDate < today hoặc (DueDate == today và đã qua 00:00:00 của ngày đó)
                    // Nếu DueDate chỉ có date (00:00:00), thì nếu today > DueDate hoặc (today == DueDate và nowLocal > dueDateLocal)
                    var dueDateOnly = dueDateLocal.Date;
                    
                    // Nếu ngày hết hạn < ngày hôm nay → overdue
                    if (dueDateOnly < todayLocal)
                        return true;
                    
                    // Nếu ngày hết hạn == ngày hôm nay, nhưng thời gian hiện tại đã qua 00:00:00 → overdue
                    if (dueDateOnly == todayLocal && nowLocal > dueDateLocal)
                        return true;
                    
                    return false;
                })
                .Select(x => x.AuditId)
                .Distinct()
                .ToList();

            // Update status cho các schedules overdue
            if (overdueAuditIds.Any())
            {
                var scheduleIds = schedules
                    .Where(x => overdueAuditIds.Contains(x.AuditId))
                    .Select(x => x.ScheduleId)
                    .ToList();

                await _context.AuditSchedules
                    .Where(x => scheduleIds.Contains(x.ScheduleId))
                    .ExecuteUpdateAsync(
                        setters => setters.SetProperty(s => s.Status, "Overdue"),
                        ct);
            }

            return overdueAuditIds;
        }

        public async Task<List<Guid>> MarkCapaDueOverdueAsync(CancellationToken ct = default)
        {
            // Lấy danh sách schedules
            var schedules = await _context.AuditSchedules.AsNoTracking()
                .Where(x =>
                    x.MilestoneName == "CAPA Due" &&
                    x.Status == "Active")
                .ToListAsync(ct);

            if (!schedules.Any())
                return new List<Guid>();

            // So sánh với thời gian hiện tại theo local timezone
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneHelper.TimeZone);
            var todayLocal = nowLocal.Date;

            var overdueAuditIds = schedules
                .Where(x =>
                {
                    // Convert DueDate từ DB sang local time để so sánh
                    DateTime dueDateLocal;
                    if (x.DueDate.Kind == DateTimeKind.Utc)
                    {
                        dueDateLocal = TimeZoneInfo.ConvertTimeFromUtc(x.DueDate, TimeZoneHelper.TimeZone);
                    }
                    else
                    {
                        // Unspecified - giả định là local time (date only)
                        dueDateLocal = x.DueDate;
                    }
                    
                    // Overdue nếu: DueDate < today hoặc (DueDate == today và đã qua 00:00:00 của ngày đó)
                    var dueDateOnly = dueDateLocal.Date;
                    
                    if (dueDateOnly < todayLocal)
                        return true;
                    
                    if (dueDateOnly == todayLocal && nowLocal > dueDateLocal)
                        return true;
                    
                    return false;
                })
                .Select(x => x.AuditId)
                .Distinct()
                .ToList();

            // Update status cho các schedules overdue
            if (overdueAuditIds.Any())
            {
                var scheduleIds = schedules
                    .Where(x => overdueAuditIds.Contains(x.AuditId))
                    .Select(x => x.ScheduleId)
                    .ToList();

                await _context.AuditSchedules
                    .Where(x => scheduleIds.Contains(x.ScheduleId))
                    .ExecuteUpdateAsync(
                        setters => setters.SetProperty(s => s.Status, "Overdue"),
                        ct);
            }

            return overdueAuditIds;
        }

        public async Task<List<Guid>> MarkDraftReportDueOverdueAsync(CancellationToken ct = default)
        {
            // Lấy danh sách schedules
            var schedules = await _context.AuditSchedules.AsNoTracking()
                .Where(x =>
                    x.MilestoneName == "Draft Report Due" &&
                    x.Status == "Active")
                .ToListAsync(ct);

            if (!schedules.Any())
                return new List<Guid>();

            // So sánh với thời gian hiện tại theo local timezone
            var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneHelper.TimeZone);
            var todayLocal = nowLocal.Date;

            var overdueAuditIds = schedules
                .Where(x =>
                {
                    // Convert DueDate từ DB sang local time để so sánh
                    DateTime dueDateLocal;
                    if (x.DueDate.Kind == DateTimeKind.Utc)
                    {
                        dueDateLocal = TimeZoneInfo.ConvertTimeFromUtc(x.DueDate, TimeZoneHelper.TimeZone);
                    }
                    else
                    {
                        // Unspecified - giả định là local time (date only)
                        dueDateLocal = x.DueDate;
                    }
                    
                    // Overdue nếu: DueDate < today hoặc (DueDate == today và đã qua 00:00:00 của ngày đó)
                    var dueDateOnly = dueDateLocal.Date;
                    
                    if (dueDateOnly < todayLocal)
                        return true;
                    
                    if (dueDateOnly == todayLocal && nowLocal > dueDateLocal)
                        return true;
                    
                    return false;
                })
                .Select(x => x.AuditId)
                .Distinct()
                .ToList();

            // Update status cho các schedules overdue
            if (overdueAuditIds.Any())
            {
                var scheduleIds = schedules
                    .Where(x => overdueAuditIds.Contains(x.AuditId))
                    .Select(x => x.ScheduleId)
                    .ToList();

                await _context.AuditSchedules
                    .Where(x => scheduleIds.Contains(x.ScheduleId))
                    .ExecuteUpdateAsync(
                        setters => setters.SetProperty(s => s.Status, "Overdue"),
                        ct);
            }

            return overdueAuditIds;
        }

        public async Task<List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>> GetDraftReportDueTomorrowAssignmentsAsync(CancellationToken ct = default)
        {
            var (start, end) = GetLocalTomorrowRangeUtc();

            var results = await _context.AuditSchedules.AsNoTracking()
                .Where(x =>
                    x.MilestoneName == "Draft Report Due" &&
                    x.Status == "Active" &&
                    x.DueDate >= start &&
                    x.DueDate < end)
                .Join(
                    _context.AuditAssignments.AsNoTracking(),
                    schedule => schedule.AuditId,
                    assignment => assignment.AuditId,
                    (schedule, assignment) => new
                    {
                        schedule.AuditId,
                        assignment.AuditorId,
                        schedule.DueDate
                    })
                .ToListAsync(ct);

            return results
                .Select(x => (x.AuditId, x.AuditorId, x.DueDate))
                .ToList();
        }

        public async Task<List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>> GetCapaDueTomorrowAssignmentsAsync(CancellationToken ct = default)
        {
            var (start, end) = GetLocalTomorrowRangeUtc();

            // Lấy schedules với CAPA Due due tomorrow
            var schedules = await _context.AuditSchedules.AsNoTracking()
                .Where(x =>
                    x.MilestoneName == "CAPA Due" &&
                    x.Status == "Active" &&
                    x.DueDate >= start &&
                    x.DueDate < end)
                .ToListAsync(ct);

            if (!schedules.Any())
                return new List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>();

            var auditIds = schedules.Select(s => s.AuditId).Distinct().ToList();

            // Lấy tất cả Findings của các audits này
            var findings = await _context.Findings.AsNoTracking()
                .Where(f => auditIds.Contains(f.AuditId))
                .ToListAsync(ct);

            if (!findings.Any())
                return new List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>();

            var findingIds = findings.Select(f => f.FindingId).ToList();

            // Lấy tất cả Actions của các findings này
            var actions = await _context.Actions.AsNoTracking()
                .Where(a => findingIds.Contains(a.FindingId))
                .ToListAsync(ct);

            // Lấy tất cả DeptIds từ findings (cả có và không có action)
            var deptIds = findings
                .Where(f => f.DeptId.HasValue)
                .Select(f => f.DeptId.Value)
                .Distinct()
                .ToList();

            // Lấy tất cả UserAccounts có RoleName = "AuditeeOwner" và DeptId trong danh sách
            var userAccounts = new List<UserAccount>();
            if (deptIds.Any())
            {
                userAccounts = await _context.UserAccounts.AsNoTracking()
                    .Where(u => u.DeptId.HasValue && deptIds.Contains(u.DeptId.Value) && u.RoleName == "AuditeeOwner")
                    .ToListAsync(ct);
            }

            // Tạo dictionary để lookup nhanh
            var actionsByFindingId = actions
                .Where(a => a.AssignedTo.HasValue)
                .GroupBy(a => a.FindingId)
                .ToDictionary(g => g.Key, g => g.First().AssignedTo.Value);

            var userAccountsByDeptId = userAccounts
                .GroupBy(u => u.DeptId.Value)
                .ToDictionary(g => g.Key, g => g.First().UserId);

            var results = new List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>();

            foreach (var schedule in schedules)
            {
                var auditFindings = findings.Where(f => f.AuditId == schedule.AuditId);

                foreach (var finding in auditFindings)
                {
                    var userIds = new HashSet<Guid>();

                    // Nếu có Action với AssignedTo, thêm AssignedTo vào danh sách
                    if (actionsByFindingId.TryGetValue(finding.FindingId, out var assignedTo))
                    {
                        userIds.Add(assignedTo);
                    }

                    // Vẫn lấy UserAccount theo DeptId (nếu có)
                    if (finding.DeptId.HasValue && userAccountsByDeptId.TryGetValue(finding.DeptId.Value, out var deptOwnerId))
                    {
                        userIds.Add(deptOwnerId);
                    }

                    // Thêm tất cả userIds vào kết quả
                    foreach (var userId in userIds)
                    {
                        results.Add((schedule.AuditId, userId, schedule.DueDate));
                    }
                }
            }

            // Loại bỏ duplicates
            return results.Distinct().ToList();
        }

        public async Task<List<(Guid AuditId, Guid AuditorId, DateTime DueDate)>> GetEvidenceDueTomorrowAssignmentsAsync(CancellationToken ct = default)
        {
            var (start, end) = GetLocalTomorrowRangeUtc();

            var results = await _context.AuditSchedules.AsNoTracking()
                .Where(x =>
                    x.MilestoneName == "Evidence Due" &&
                    x.Status == "Active" &&
                    x.DueDate >= start &&
                    x.DueDate < end)
                .Join(
                    _context.AuditAssignments.AsNoTracking(),
                    schedule => schedule.AuditId,
                    assignment => assignment.AuditId,
                    (schedule, assignment) => new
                    {
                        schedule.AuditId,
                        assignment.AuditorId,
                        schedule.DueDate
                    })
                .ToListAsync(ct);

            return results
                .Select(x => (x.AuditId, x.AuditorId, x.DueDate))
                .ToList();
        }

        private static DateTime GetStartOfTodayUtc()
        {
            var nowUtc = DateTime.UtcNow;
            var todayLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, TimeZoneHelper.TimeZone).Date;
            return TimeZoneInfo.ConvertTimeToUtc(todayLocal, TimeZoneHelper.TimeZone);
        }

        private static (DateTime StartUtc, DateTime EndUtc) GetLocalTomorrowRangeUtc()
        {
            var nowUtc = DateTime.UtcNow;
            var todayLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, TimeZoneHelper.TimeZone).Date;
            var startTomorrowLocal = todayLocal.AddDays(1);
            var endTomorrowLocal = startTomorrowLocal.AddDays(1);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(startTomorrowLocal, TimeZoneHelper.TimeZone);
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(endTomorrowLocal, TimeZoneHelper.TimeZone);
            return (startUtc, endUtc);
        }
    }
}

