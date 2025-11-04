using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.DepartmentHeadRepositories
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
    }

}
