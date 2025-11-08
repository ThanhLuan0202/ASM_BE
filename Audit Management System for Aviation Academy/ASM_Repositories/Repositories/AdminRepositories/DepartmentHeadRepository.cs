using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.DepartmentHeadDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.AdminRepositories
{
    public class DepartmentHeadRepository : IDepartmentHeadRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public DepartmentHeadRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewDepartmentHead>> GetAllAsync()
        {
            var data = await _context.DepartmentHeads
                .Include(x => x.Dept)
                .Include(x => x.User)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ViewDepartmentHead>>(data);
        }

        public async Task<ViewDepartmentHead?> GetByIdAsync(Guid deptHeadId)
        {
            var entity = await _context.DepartmentHeads
                .Include(x => x.Dept)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.DeptHeadId == deptHeadId);
            return entity == null ? null : _mapper.Map<ViewDepartmentHead>(entity);
        }

        public async Task<ViewDepartmentHead> CreateAsync(CreateDepartmentHead dto)
        {
            var deptExists = await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId);
            if (!deptExists)
                throw new InvalidOperationException($"Department with ID {dto.DeptId} does not exist.");

            var userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

            var exists = await _context.DepartmentHeads
                .AnyAsync(x => x.DeptId == dto.DeptId && x.UserId == dto.UserId);
            if (exists)
                throw new InvalidOperationException($"This user is already assigned as head of this department.");

            var entity = _mapper.Map<DepartmentHead>(dto);
            entity.DeptHeadId = Guid.NewGuid();
            
            if (string.IsNullOrWhiteSpace(entity.Status))
                entity.Status = "Active";

            _context.DepartmentHeads.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.DepartmentHeads
                .Include(x => x.Dept)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.DeptHeadId == entity.DeptHeadId);

            return _mapper.Map<ViewDepartmentHead>(created);
        }

        public async Task<ViewDepartmentHead?> UpdateAsync(Guid deptHeadId, UpdateDepartmentHead dto)
        {
            var entity = await _context.DepartmentHeads
                .FirstOrDefaultAsync(x => x.DeptHeadId == deptHeadId);

            if (entity == null) return null;

            var deptExists = await _context.Departments.AnyAsync(d => d.DeptId == dto.DeptId);
            if (!deptExists)
                throw new InvalidOperationException($"Department with ID {dto.DeptId} does not exist.");

            var userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

            var exists = await _context.DepartmentHeads
                .AnyAsync(x => x.DeptId == dto.DeptId && x.UserId == dto.UserId && x.DeptHeadId != deptHeadId);
            if (exists)
                throw new InvalidOperationException($"This user is already assigned as head of this department.");

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            var updated = await _context.DepartmentHeads
                .Include(x => x.Dept)
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.DeptHeadId == deptHeadId);

            return _mapper.Map<ViewDepartmentHead>(updated);
        }

        public async Task<bool> DeleteAsync(Guid deptHeadId)
        {
            var entity = await _context.DepartmentHeads.FindAsync(deptHeadId);
            if (entity == null) return false;

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

