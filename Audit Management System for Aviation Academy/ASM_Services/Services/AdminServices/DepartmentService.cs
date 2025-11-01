using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.AdminInterfaces.AdminRepositories;
using ASM_Repositories.Models;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Services.Interfaces.AdminInterfaces.AdminServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.AdminInterfaces.AdminServices
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentService(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewDepartment>> GetAllDepartmentsAsync() => _repo.GetAllDepartmentsAsync();
        public Task<ViewDepartment?> GetDepartmentByIdAsync(int id) => _repo.GetDepartmentByIdAsync(id);
        public Task<ViewDepartment> CreateDepartmentAsync(CreateDepartment dto) => _repo.CreateDepartmentAsync(dto);
        public Task<ViewDepartment?> UpdateDepartmentAsync(int id, UpdateDepartment dto) => _repo.UpdateDepartmentAsync(id, dto);
        public Task<bool> DeleteDepartmentAsync(int id) => _repo.DeleteDepartmentAsync(id);
    }

}
