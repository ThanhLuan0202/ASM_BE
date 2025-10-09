using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentService(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ViewDepartment>> GetAllDepartmentsAsync()
        {
            var departments = await _repo.GetAllAsync();
            return departments.Select(d => new ViewDepartment
            {
                DeptId = d.DeptId,
                Name = d.Name ?? "",
                Code = d.Code,
                Description = d.Description,
                CreatedAt = d.CreatedAt
            });
        }

        public async Task<ViewDepartment?> GetDepartmentByIdAsync(int id)
        {
            var dept = await _repo.GetByIdAsync(id);
            if (dept == null) return null;

            return new ViewDepartment
            {
                DeptId = dept.DeptId,
                Name = dept.Name ?? "",
                Code = dept.Code,
                Description = dept.Description,
                CreatedAt = dept.CreatedAt
            };
        }

        public async Task<ViewDepartment> CreateDepartmentAsync(CreateDepartment dto)
        {
            var dept = new Department
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                CreatedAt = DateTime.Now
            };

            await _repo.AddAsync(dept);

            return new ViewDepartment
            {
                DeptId = dept.DeptId,
                Name = dept.Name ?? "",
                Code = dept.Code,
                Description = dept.Description,
                CreatedAt = dept.CreatedAt
            };
        }

        public async Task<ViewDepartment?> UpdateDepartmentAsync(int id, UpdateDepartment dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Code = dto.Code;   
            existing.Description = dto.Description;

            await _repo.UpdateAsync(existing);

            return new ViewDepartment
            {
                DeptId = existing.DeptId,
                Name = existing.Name ?? "",
                Code = existing.Code,
                Description = existing.Description,
                CreatedAt = existing.CreatedAt
            };
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            await _repo.DeleteAsync(id);
            return true;
        }
    }

}
