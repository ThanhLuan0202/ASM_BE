using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.DepartmentSensitiveAreaDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class DepartmentSensitiveAreaRepository : IDepartmentSensitiveAreaRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public DepartmentSensitiveAreaRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewDepartmentSensitiveArea>> GetAllAsync()
        {
            var entities = await _context.DepartmentSensitiveAreas
                .Include(x => x.Dept)
                .OrderBy(x => x.DeptId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewDepartmentSensitiveArea>>(entities);
        }

        public async Task<ViewDepartmentSensitiveArea?> GetByIdAsync(Guid id)
        {
            var entity = await _context.DepartmentSensitiveAreas
                .Include(x => x.Dept)
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? null : _mapper.Map<ViewDepartmentSensitiveArea>(entity);
        }

        public async Task<ViewDepartmentSensitiveArea?> GetByDeptIdAsync(int deptId)
        {
            var entity = await _context.DepartmentSensitiveAreas
                .Include(x => x.Dept)
                .FirstOrDefaultAsync(x => x.DeptId == deptId);

            return entity == null ? null : _mapper.Map<ViewDepartmentSensitiveArea>(entity);
        }

        public async Task<ViewDepartmentSensitiveArea> CreateAsync(CreateDepartmentSensitiveArea dto, string createdBy)
        {
            // Check if department already has sensitive area
            var exists = await _context.DepartmentSensitiveAreas.AnyAsync(x => x.DeptId == dto.DeptId);
            if (exists)
                throw new InvalidOperationException($"Department with ID {dto.DeptId} already has sensitive area configuration");

            // Validate Department exists
            var deptExists = await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId);
            if (!deptExists)
                throw new InvalidOperationException($"Department with ID {dto.DeptId} does not exist");

            var entity = _mapper.Map<DepartmentSensitiveArea>(dto);
            entity.CreatedBy = createdBy;

            _context.DepartmentSensitiveAreas.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.DepartmentSensitiveAreas
                .Include(x => x.Dept)
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return _mapper.Map<ViewDepartmentSensitiveArea>(created);
        }

        public async Task<ViewDepartmentSensitiveArea?> UpdateAsync(Guid id, UpdateDepartmentSensitiveArea dto)
        {
            var entity = await _context.DepartmentSensitiveAreas
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            _mapper.Map(dto, entity);

            _context.DepartmentSensitiveAreas.Update(entity);
            await _context.SaveChangesAsync();

            var updated = await _context.DepartmentSensitiveAreas
                .Include(x => x.Dept)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<ViewDepartmentSensitiveArea>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _context.DepartmentSensitiveAreas
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            _context.DepartmentSensitiveAreas.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByDeptIdAsync(int deptId)
        {
            return await _context.DepartmentSensitiveAreas.AnyAsync(x => x.DeptId == deptId);
        }
    }
}
