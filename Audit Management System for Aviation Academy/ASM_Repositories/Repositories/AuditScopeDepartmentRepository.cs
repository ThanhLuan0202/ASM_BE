using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AuditScopeDepartmentRepository : IAuditScopeDepartmentRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AuditScopeDepartmentRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAuditScopeDepartment>> GetAllAsync()
        {
            var list = await _context.AuditScopeDepartments
                .Where(x => x.Status == "Active")
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAuditScopeDepartment>>(list);
        }

        public async Task<ViewAuditScopeDepartment?> GetByIdAsync(Guid id)
        {
            var entity = await _context.AuditScopeDepartments
                .FirstOrDefaultAsync(x => x.AuditScopeId == id);

            return entity == null ? null : _mapper.Map<ViewAuditScopeDepartment>(entity);
        }

        public async Task<ViewAuditScopeDepartment> AddAsync(CreateAuditScopeDepartment dto)
        {
            try
            {
                bool auditExists = await _context.Audits.AnyAsync(a => a.AuditId == dto.AuditId);
                bool deptExists = await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId);

                if (!auditExists)
                    throw new ArgumentException($"AuditId '{dto.AuditId}' does not exist.");
                if (!deptExists)
                    throw new ArgumentException($"DeptId '{dto.DeptId}' does not exist.");

                bool duplicate = await _context.AuditScopeDepartments
                    .AnyAsync(x => x.AuditId == dto.AuditId && x.DeptId == dto.DeptId);

                if (duplicate)
                    throw new ArgumentException("This Audit and Department mapping already exists.");

                var entity = _mapper.Map<AuditScopeDepartment>(dto);
                _context.AuditScopeDepartments.Add(entity);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewAuditScopeDepartment>(entity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[AuditScopeDepartmentRepository.AddAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while creating AuditScopeDepartment.", ex);
            }
        }

        public async Task<ViewAuditScopeDepartment?> UpdateAsync(Guid id, UpdateAuditScopeDepartment dto)
        {
            var entity = await _context.AuditScopeDepartments.FirstOrDefaultAsync(x => x.AuditScopeId == id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            _context.AuditScopeDepartments.Update(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAuditScopeDepartment>(entity);
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.AuditScopeDepartments.FirstOrDefaultAsync(x => x.AuditScopeId == id);
            if (entity == null || entity.Status == "Inactive") return false;

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AuditScopeDepartment>> GetAuditScopeDepartmentsAsync(Guid auditId)
        => await _context.AuditScopeDepartments.Include(f => f.Dept)
            .Where(f => f.AuditId == auditId).ToListAsync();

        public async Task UpdateScopeDepartmentsAsync(Guid auditId, List<UpdateAuditScopeDepartment>? list)
        {
            if (list == null || !list.Any())
                return; // Không có gì để update, bỏ qua

            // Xóa scope cũ
            var existing = _context.AuditScopeDepartments
                .Where(x => x.AuditId == auditId);
            _context.AuditScopeDepartments.RemoveRange(existing);

            // Thêm scope mới
            foreach (var item in list)
            {
                var entity = _mapper.Map<AuditScopeDepartment>(item);
                entity.AuditId = auditId;
                await _context.AuditScopeDepartments.AddAsync(entity);
            }
        }

        public async Task<IEnumerable<DepartmentInfoDto>> GetDepartmentsByAuditIdAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty");

            var departments = await _context.AuditScopeDepartments
                .Where(asc => asc.AuditId == auditId && asc.Status == "Active")
                .Include(asc => asc.Dept)
                .Select(asc => new Interfaces.DepartmentInfoDto
                {
                    DeptId = asc.DeptId,
                    Name = asc.Dept != null ? asc.Dept.Name : string.Empty
                })
                .Distinct()
                .OrderBy(d => d.Name)
                .ToListAsync();

            return departments;
        }

    }

}
