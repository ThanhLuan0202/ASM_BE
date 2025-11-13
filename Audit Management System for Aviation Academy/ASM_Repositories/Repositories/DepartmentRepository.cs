using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.DepartmentDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public DepartmentRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewDepartment>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments.ToListAsync();
            return _mapper.Map<IEnumerable<ViewDepartment>>(departments);
        }

        public async Task<ViewDepartment?> GetDepartmentByIdAsync(int id)
        {
            var dept = await _context.Departments.FindAsync(id);
            return dept == null ? null : _mapper.Map<ViewDepartment>(dept);
        }

        public async Task<ViewDepartment> CreateDepartmentAsync(CreateDepartment dto)
        {
            var dept = _mapper.Map<Department>(dto);
            dept.CreatedAt = DateTime.Now;

            _context.Departments.Add(dept);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewDepartment>(dept);
        }

        public async Task<ViewDepartment?> UpdateDepartmentAsync(int id, UpdateDepartment dto)
        {
            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewDepartment>(existing);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) return false;

            _context.Departments.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Departments.AnyAsync(d => d.DeptId == id);
        }


    }

}
