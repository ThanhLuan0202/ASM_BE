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
                .Include(x => x.LevelNavigation)
                .Include(x => x.CreatedByNavigation)
                .OrderBy(x => x.DeptId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewDepartmentSensitiveArea>>(entities);
        }

        public async Task<ViewDepartmentSensitiveArea?> GetByIdAsync(Guid id)
        {
            var entity = await _context.DepartmentSensitiveAreas
                .Include(x => x.Dept)
                .Include(x => x.LevelNavigation)
                .Include(x => x.CreatedByNavigation)
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? null : _mapper.Map<ViewDepartmentSensitiveArea>(entity);
        }

        public async Task<ViewDepartmentSensitiveArea?> GetByDeptIdAsync(int deptId)
        {
            var entity = await _context.DepartmentSensitiveAreas
                .Include(x => x.Dept)
                .Include(x => x.LevelNavigation)
                .Include(x => x.CreatedByNavigation)
                .FirstOrDefaultAsync(x => x.DeptId == deptId);

            return entity == null ? null : _mapper.Map<ViewDepartmentSensitiveArea>(entity);
        }

        public async Task<ViewDepartmentSensitiveArea> CreateAsync(CreateDepartmentSensitiveArea dto, Guid? createdBy)
        {
            // Validate Department exists
            var deptExists = await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId);
            if (!deptExists)
                throw new InvalidOperationException($"Department with ID {dto.DeptId} does not exist");

            // Validate không được trùng tên khu vực trong cùng một phòng
            if (!string.IsNullOrWhiteSpace(dto.SensitiveArea))
            {
                var duplicateExists = await _context.DepartmentSensitiveAreas
                    .AnyAsync(x => x.DeptId == dto.DeptId && 
                                   x.SensitiveAreas != null && 
                                   x.SensitiveAreas.Trim().ToLower() == dto.SensitiveArea.Trim().ToLower());
                
                if (duplicateExists)
                    throw new InvalidOperationException($"Sensitive area '{dto.SensitiveArea}' already exists for department {dto.DeptId}");
            }

            // Validate Level exists if provided
            if (!string.IsNullOrEmpty(dto.Level))
            {
                var levelExists = await _context.SensitiveAreaLevels.AnyAsync(l => l.Level == dto.Level);
                if (!levelExists)
                    throw new InvalidOperationException($"Level '{dto.Level}' does not exist");
            }

            // Validate CreatedBy user exists if provided
            if (createdBy.HasValue)
            {
                var userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == createdBy.Value);
                if (!userExists)
                    throw new InvalidOperationException($"User with ID {createdBy} does not exist");
            }

            var entity = _mapper.Map<DepartmentSensitiveArea>(dto);
            entity.CreatedBy = createdBy;

            _context.DepartmentSensitiveAreas.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.DepartmentSensitiveAreas
                .Include(x => x.Dept)
                .Include(x => x.LevelNavigation)
                .Include(x => x.CreatedByNavigation)
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return _mapper.Map<ViewDepartmentSensitiveArea>(created);
        }

        public async Task<ViewDepartmentSensitiveArea?> UpdateAsync(Guid id, UpdateDepartmentSensitiveArea dto)
        {
            var entity = await _context.DepartmentSensitiveAreas
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return null;

            // Validate không được trùng tên khu vực trong cùng một phòng (trừ chính bản ghi đang update)
            if (!string.IsNullOrWhiteSpace(dto.SensitiveArea))
            {
                var duplicateExists = await _context.DepartmentSensitiveAreas
                    .AnyAsync(x => x.Id != id && 
                                   x.DeptId == entity.DeptId && 
                                   x.SensitiveAreas != null && 
                                   x.SensitiveAreas.Trim().ToLower() == dto.SensitiveArea.Trim().ToLower());
                
                if (duplicateExists)
                    throw new InvalidOperationException($"Sensitive area '{dto.SensitiveArea}' already exists for department {entity.DeptId}");
            }

            // Validate Level exists if provided
            if (!string.IsNullOrEmpty(dto.Level))
            {
                var levelExists = await _context.SensitiveAreaLevels.AnyAsync(l => l.Level == dto.Level);
                if (!levelExists)
                    throw new InvalidOperationException($"Level '{dto.Level}' does not exist");
            }

            _mapper.Map(dto, entity);

            _context.DepartmentSensitiveAreas.Update(entity);
            await _context.SaveChangesAsync();

            var updated = await _context.DepartmentSensitiveAreas
                .Include(x => x.Dept)
                .Include(x => x.LevelNavigation)
                .Include(x => x.CreatedByNavigation)
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
