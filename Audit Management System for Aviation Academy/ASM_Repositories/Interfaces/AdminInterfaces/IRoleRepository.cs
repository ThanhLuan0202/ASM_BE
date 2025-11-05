using ASM_Repositories.Models.RoleDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.AdminInterfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<ViewRole>> GetAllAsync();
        Task<ViewRole?> GetByIdAsync(string roleName);
        Task<ViewRole> AddAsync(CreateRole dto);
        Task<ViewRole?> UpdateAsync(string roleName, UpdateRole dto);
        Task<bool> DeleteAsync(string roleName);
    }
}
