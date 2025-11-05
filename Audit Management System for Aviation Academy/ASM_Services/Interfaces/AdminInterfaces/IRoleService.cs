using ASM_Repositories.Models.RoleDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.AdminInterfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<ViewRole>> GetAllAsync();
        Task<ViewRole?> GetByIdAsync(string roleName);
        Task<ViewRole> CreateAsync(CreateRole dto);
        Task<ViewRole?> UpdateAsync(string roleName, UpdateRole dto);
        Task<bool> DeleteAsync(string roleName);
    }
}
