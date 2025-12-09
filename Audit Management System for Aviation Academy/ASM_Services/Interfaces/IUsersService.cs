using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.UsersDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.AdminInterfaces
{
    public interface IUsersService
    {
        Task<IEnumerable<ViewUser>> GetAllAsync();
        Task<ViewUser> GetByIdAsync(Guid id);
        Task<ViewUser> CreateAsync(CreateUser dto, Guid userId);
        Task<ViewUser> UpdateAsync(Guid id, UpdateUser dto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task<IEnumerable<ViewUser>> GetByDeptIdAsync(int deptId);
        Task<Guid?> GetLeadAuditorIdAsync();
    }
}
