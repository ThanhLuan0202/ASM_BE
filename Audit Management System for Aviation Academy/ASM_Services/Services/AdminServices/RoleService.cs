using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.RoleDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services.AdminServices
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;
        public RoleService(IRoleRepository repo) => _repo = repo;

        public Task<IEnumerable<ViewRole>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewRole?> GetByIdAsync(string roleName) => _repo.GetByIdAsync(roleName);
        public Task<ViewRole> CreateAsync(CreateRole dto) => _repo.AddAsync(dto);
        public Task<ViewRole?> UpdateAsync(string roleName, UpdateRole dto) => _repo.UpdateAsync(roleName, dto);
        public Task<bool> DeleteAsync(string roleName) => _repo.DeleteAsync(roleName);
    }
}
