using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.DepartmentHeadDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services.AdminServices
{
    public class DepartmentHeadService : IDepartmentHeadService
    {
        private readonly IDepartmentHeadRepository _repo;

        public DepartmentHeadService(IDepartmentHeadRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewDepartmentHead>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewDepartmentHead?> GetByIdAsync(Guid deptHeadId) => _repo.GetByIdAsync(deptHeadId);
        public Task<ViewDepartmentHead> CreateAsync(CreateDepartmentHead dto) => _repo.CreateAsync(dto);
        public Task<ViewDepartmentHead?> UpdateAsync(Guid deptHeadId, UpdateDepartmentHead dto) => _repo.UpdateAsync(deptHeadId, dto);
        public Task<bool> DeleteAsync(Guid deptHeadId) => _repo.DeleteAsync(deptHeadId);
    }
}

