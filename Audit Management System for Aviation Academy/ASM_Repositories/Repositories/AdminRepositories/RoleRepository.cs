using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.RoleDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.AdminRepositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public RoleRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewRole>> GetAllAsync()
        {
            var data = await _context.Roles.ToListAsync();
            return _mapper.Map<IEnumerable<ViewRole>>(data);
        }

        public async Task<ViewRole?> GetByIdAsync(string roleName)
        {
            var entity = await _context.Roles.FindAsync(roleName);
            return _mapper.Map<ViewRole?>(entity);
        }

        public async Task<ViewRole> AddAsync(CreateRole dto)
        {
            var entity = _mapper.Map<Role>(dto);
            _context.Roles.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ViewRole>(entity);
        }

        public async Task<ViewRole?> UpdateAsync(string roleName, UpdateRole dto)
        {
            var entity = await _context.Roles.FindAsync(roleName);
            if (entity == null) return null;

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<ViewRole>(entity);
        }

        public async Task<bool> DeleteAsync(string roleName)
        {
            var entity = await _context.Roles.FindAsync(roleName);
            if (entity == null) return false;

            _context.Roles.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
