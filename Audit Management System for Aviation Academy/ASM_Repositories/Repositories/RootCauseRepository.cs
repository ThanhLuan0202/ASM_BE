using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.RootCauseDTO;
using ASM_Repositories.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class RootCauseRepository : Repository<RootCause>, IRootCauseRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _DbContext;
        private readonly IMapper _mapper;

        public RootCauseRepository(AuditManagementSystemForAviationAcademyContext DbContext, IMapper mapper)
        {
            _DbContext = DbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewRootCause>> GetAllAsync()
        {
            var list = await _DbContext.RootCauses
                .Include(r => r.Dept)
                .Where(r => r.Status != "Inactive")
                .ToListAsync();
<<<<<<< HEAD

=======
            
>>>>>>> 84313ad820760eb11d01445964f27b91148ce098
            return list.Select(r => new ViewRootCause
            {
                RootCauseId = r.RootCauseId,
                Name = r.Name,
                Category = r.Category,
                Status = r.Status,
                Description = r.Description,
                DeptId = r.DeptId,
                DepartmentName = r.Dept?.Name ?? string.Empty,
                FindingId = r.FindingId
            });
        }

        public async Task<IEnumerable<ViewRootCause>> GetByStatusAsync(string status)
        {
            var list = await _DbContext.RootCauses
                .Include(r => r.Dept)
                .Where(r => r.Status == status)
                .ToListAsync();
<<<<<<< HEAD

=======
            
>>>>>>> 84313ad820760eb11d01445964f27b91148ce098
            return list.Select(r => new ViewRootCause
            {
                RootCauseId = r.RootCauseId,
                Name = r.Name,
                Category = r.Category,
                Status = r.Status,
                Description = r.Description,
                DeptId = r.DeptId,
                DepartmentName = r.Dept?.Name ?? string.Empty,
                FindingId = r.FindingId
            });
        }

        public async Task<IEnumerable<ViewRootCause>> GetByCategoryAsync(string category)
        {
            var list = await _DbContext.RootCauses
                .Include(r => r.Dept)
                .Where(r => r.Category == category && r.Status != "Inactive")
                .ToListAsync();
<<<<<<< HEAD

=======
            
>>>>>>> 84313ad820760eb11d01445964f27b91148ce098
            return list.Select(r => new ViewRootCause
            {
                RootCauseId = r.RootCauseId,
                Name = r.Name,
                Category = r.Category,
                Status = r.Status,
                Description = r.Description,
                DeptId = r.DeptId,
                DepartmentName = r.Dept?.Name ?? string.Empty,
                FindingId = r.FindingId
            });
        }

        public async Task<IEnumerable<ViewRootCause>> GetByDeptIdAsync(int deptId)
        {
            var list = await _DbContext.RootCauses
                .Include(r => r.Dept)
                .Where(r => r.DeptId == deptId && r.Status != "Inactive")
                .ToListAsync();
<<<<<<< HEAD

=======
            
>>>>>>> 84313ad820760eb11d01445964f27b91148ce098
            return list.Select(r => new ViewRootCause
            {
                RootCauseId = r.RootCauseId,
                Name = r.Name,
                Category = r.Category,
                Status = r.Status,
                Description = r.Description,
                DeptId = r.DeptId,
                DepartmentName = r.Dept?.Name ?? string.Empty,
                FindingId = r.FindingId
            });
        }

        public async Task<IEnumerable<ViewRootCause>> GetByFindingIdAsync(Guid findingId)
        {
            var list = await _DbContext.RootCauses
                .Include(r => r.Dept)
                .Where(r => r.FindingId == findingId && r.Status != "Inactive")
                .ToListAsync();
<<<<<<< HEAD

=======
            
>>>>>>> 84313ad820760eb11d01445964f27b91148ce098
            return list.Select(r => new ViewRootCause
            {
                RootCauseId = r.RootCauseId,
                Name = r.Name,
                Category = r.Category,
                Status = r.Status,
                Description = r.Description,
                DeptId = r.DeptId,
                DepartmentName = r.Dept?.Name ?? string.Empty,
                FindingId = r.FindingId
            });
        }

        public async Task<ViewRootCause?> GetByIdAsync(int id)
        {
            var entity = await _DbContext.RootCauses
                .Include(r => r.Dept)
                .FirstOrDefaultAsync(r => r.RootCauseId == id);
<<<<<<< HEAD

            if (entity == null) return null;

=======
            
            if (entity == null) return null;
            
>>>>>>> 84313ad820760eb11d01445964f27b91148ce098
            return new ViewRootCause
            {
                RootCauseId = entity.RootCauseId,
                Name = entity.Name,
                Category = entity.Category,
                Status = entity.Status,
                Description = entity.Description,
                DeptId = entity.DeptId,
                DepartmentName = entity.Dept?.Name ?? string.Empty,
                FindingId = entity.FindingId
            };
        }

        public async Task<ViewRootCause> CreateAsync(CreateRootCause dto)
        {
            var entity = _mapper.Map<RootCause>(dto);
            _DbContext.RootCauses.Add(entity);
            await _DbContext.SaveChangesAsync();
<<<<<<< HEAD

            // Reload with Department to get DepartmentName
            await _DbContext.Entry(entity).Reference(r => r.Dept).LoadAsync();

=======
            
            // Reload with Department to get DepartmentName
            await _DbContext.Entry(entity).Reference(r => r.Dept).LoadAsync();
            
>>>>>>> 84313ad820760eb11d01445964f27b91148ce098
            return new ViewRootCause
            {
                RootCauseId = entity.RootCauseId,
                Name = entity.Name,
                Category = entity.Category,
                Status = entity.Status,
                Description = entity.Description,
                DeptId = entity.DeptId,
                DepartmentName = entity.Dept?.Name ?? string.Empty,
                FindingId = entity.FindingId
            };
        }

        public async Task<ViewRootCause?> UpdateAsync(int id, UpdateRootCause dto)
        {
            var existing = await _DbContext.RootCauses
                .Include(r => r.Dept)
                .FirstOrDefaultAsync(r => r.RootCauseId == id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _DbContext.SaveChangesAsync();
<<<<<<< HEAD

=======
            
>>>>>>> 84313ad820760eb11d01445964f27b91148ce098
            return new ViewRootCause
            {
                RootCauseId = existing.RootCauseId,
                Name = existing.Name,
                Category = existing.Category,
                Status = existing.Status,
                Description = existing.Description,
                DeptId = existing.DeptId,
                DepartmentName = existing.Dept?.Name ?? string.Empty,
                FindingId = existing.FindingId
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _DbContext.RootCauses.FindAsync(id);
            if (existing == null) return false;

            existing.Status = "Inactive";
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _DbContext.RootCauses.AnyAsync(x => x.RootCauseId == id);
        }

        public async Task<Dictionary<int, string>> GetRootCausesAsync(List<int> rootIds)
        => await _DbContext.RootCauses.Where(r => rootIds.Contains(r.RootCauseId))
            .ToDictionaryAsync(r => r.RootCauseId, r => r.Name);
    }
}
