using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditChecklistItemDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditChecklistItemRepository : Repository<AuditChecklistItem>, IAuditChecklistItemRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IMapper _mapper;

        public AuditChecklistItemRepository(AuditManagementSystemForAviationAcademyContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetByAuditIdAsync(Guid auditId)
        {
            var list = await _DbContext.AuditChecklistItems
                .Where(x => x.AuditId == auditId)
                .OrderBy(x => x.Section)
                .ThenBy(x => x.Order)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ViewAuditChecklistItem>>(list);
        }

        public async Task<ViewAuditChecklistItem?> GetByIdAsync(Guid auditItemId)
        {
            var entity = await _DbContext.AuditChecklistItems.FindAsync(auditItemId);
            return entity == null ? null : _mapper.Map<ViewAuditChecklistItem>(entity);
        }

        public async Task<ViewAuditChecklistItem> CreateAsync(CreateAuditChecklistItem dto)
        {
            var auditExists = await _DbContext.Audits.AnyAsync(a => a.AuditId == dto.AuditId);
            if (!auditExists) throw new InvalidOperationException($"Audit {dto.AuditId} not found");

            var entity = _mapper.Map<AuditChecklistItem>(dto);
            entity.AuditItemId = Guid.NewGuid();

            _DbContext.AuditChecklistItems.Add(entity);
            await _DbContext.SaveChangesAsync();
            return _mapper.Map<ViewAuditChecklistItem>(entity);
        }

        public async Task<ViewAuditChecklistItem?> UpdateAsync(Guid auditItemId, UpdateAuditChecklistItem dto)
        {
            var existing = await _DbContext.AuditChecklistItems.FindAsync(auditItemId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _DbContext.SaveChangesAsync();
            return _mapper.Map<ViewAuditChecklistItem>(existing);
        }

        public async Task<bool> DeleteAsync(Guid auditItemId)
        {
            var existing = await _DbContext.AuditChecklistItems.FindAsync(auditItemId);
            if (existing == null) return false;

            _DbContext.AuditChecklistItems.Remove(existing);
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetBySectionAsync(int departmentId)
        {
            if (departmentId <= 0)
                throw new ArgumentException("DepartmentId must be greater than zero.");

            var department = await _DbContext.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptId == departmentId);

            if (department == null)
                throw new InvalidOperationException($"Department with ID '{departmentId}' was not found.");

            if (!string.Equals(department.Status, "Active", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Department '{department.Name}' is not active.");

            var items = await _DbContext.AuditChecklistItems
                .Where(aci => aci.Section == department.Name)
                .OrderBy(aci => aci.Order)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditChecklistItem>>(items);
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> GetByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            // Lấy tất cả AuditAssignment có AuditorId = userId
            var auditAssignments = await _DbContext.AuditAssignments
                .Where(aa => aa.AuditorId == userId)
                .Select(aa => aa.DeptId)
                .Distinct()
                .ToListAsync();

            if (!auditAssignments.Any())
                return new List<ViewAuditChecklistItem>();

            // Lấy tên các Department từ các DeptId
            var departmentNames = await _DbContext.Departments
                .Where(d => auditAssignments.Contains(d.DeptId) && d.Status == "Active")
                .Select(d => d.Name)
                .ToListAsync();

            if (!departmentNames.Any())
                return new List<ViewAuditChecklistItem>();

            // Lấy tất cả AuditChecklistItem có Section trùng với các Name đã lấy được
            var items = await _DbContext.AuditChecklistItems
                .Where(aci => departmentNames.Contains(aci.Section))
                .OrderBy(aci => aci.Section)
                .ThenBy(aci => aci.Order)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditChecklistItem>>(items);
        }

        public async Task<ViewAuditChecklistItem?> SetCompliantAsync(Guid auditItemId)
        {
            if (auditItemId == Guid.Empty)
                throw new ArgumentException("AuditItemId cannot be empty");

            var existing = await _DbContext.AuditChecklistItems
                .AsTracking()
                .FirstOrDefaultAsync(x => x.AuditItemId == auditItemId);

            if (existing == null)
                return null;

            existing.Status = "Compliant";
            await _DbContext.SaveChangesAsync();

            return _mapper.Map<ViewAuditChecklistItem>(existing);
        }

        public async Task<ViewAuditChecklistItem?> SetNonCompliantAsync(Guid auditItemId)
        {
            if (auditItemId == Guid.Empty)
                throw new ArgumentException("AuditItemId cannot be empty");

            var existing = await _DbContext.AuditChecklistItems
                .AsTracking()
                .FirstOrDefaultAsync(x => x.AuditItemId == auditItemId);

            if (existing == null)
                return null;

            existing.Status = "NonCompliant";
            await _DbContext.SaveChangesAsync();

            return _mapper.Map<ViewAuditChecklistItem>(existing);
        }

        public async Task<IEnumerable<ViewAuditChecklistItem>> CreateFromTemplateAsync(Guid auditId, int deptId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty");

            if (deptId <= 0)
                throw new ArgumentException("DeptId must be greater than zero");

            // 1. Lấy Audit để lấy TemplateId
            var audit = await _DbContext.Audits
                .FirstOrDefaultAsync(a => a.AuditId == auditId);

            if (audit == null)
                throw new InvalidOperationException($"Audit with ID {auditId} not found");

            if (audit.TemplateId == null || audit.TemplateId == Guid.Empty)
                throw new InvalidOperationException($"Audit {auditId} does not have a TemplateId");

            // 2. Lấy Department name
            var department = await _DbContext.Departments
                .FirstOrDefaultAsync(d => d.DeptId == deptId);

            if (department == null)
                throw new InvalidOperationException($"Department with ID {deptId} not found");

            string sectionName = department.Name;

            // 3. Lấy tất cả ChecklistItem theo TemplateId
            var checklistItems = await _DbContext.ChecklistItems
                .Where(ci => ci.TemplateId == audit.TemplateId)
                .OrderBy(ci => ci.Order)
                .ToListAsync();

            if (!checklistItems.Any())
                throw new InvalidOperationException($"No ChecklistItems found for TemplateId {audit.TemplateId}");

            // 4. Tạo các AuditChecklistItem từ ChecklistItem
            var auditChecklistItems = new List<AuditChecklistItem>();

            foreach (var checklistItem in checklistItems)
            {
                var auditChecklistItem = new AuditChecklistItem
                {
                    AuditItemId = Guid.NewGuid(),
                    AuditId = auditId,
                    QuestionTextSnapshot = checklistItem.QuestionText,
                    Section = sectionName,
                    Order = checklistItem.Order,
                    Status = "Active",
                    Comment = null
                };

                auditChecklistItems.Add(auditChecklistItem);
            }

            // 5. Lưu vào database
            _DbContext.AuditChecklistItems.AddRange(auditChecklistItems);
            await _DbContext.SaveChangesAsync();

            // 6. Trả về kết quả
            return _mapper.Map<IEnumerable<ViewAuditChecklistItem>>(auditChecklistItems);
        }

        public async Task UpdateStatusToArchivedAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty.");

            var entities = await _context.AuditChecklistItems
                .Where(a => a.AuditId == auditId)
                .ToListAsync();

            if (!entities.Any())
                throw new InvalidOperationException($"No AuditChecklistItem found for AuditId '{auditId}'.");

            foreach (var entity in entities)
            {
                entity.Status = "Archived";
                _context.Entry(entity).Property(x => x.Status).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateChecklistItemsAsync(Guid auditId, List<UpdateAuditChecklistItem>? list)
        {
            if (list == null || !list.Any())
                return; // Không có gì để update, bỏ qua

            // Xóa checklist items cũ
            var existing = _DbContext.AuditChecklistItems
                .Where(x => x.AuditId == auditId);
            _DbContext.AuditChecklistItems.RemoveRange(existing);

            // Thêm checklist items mới
            foreach (var item in list)
            {
                var entity = _mapper.Map<AuditChecklistItem>(item);
                entity.AuditItemId = Guid.NewGuid();
                entity.AuditId = auditId;
                if (string.IsNullOrEmpty(entity.Status))
                    entity.Status = "Active";
                await _DbContext.AuditChecklistItems.AddAsync(entity);
            }
        }


    }
}
