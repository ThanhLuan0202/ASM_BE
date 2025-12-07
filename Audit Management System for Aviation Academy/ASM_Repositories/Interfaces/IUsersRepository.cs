using ASM_Repositories.Models.DepartmentDTO;
using ASM_Repositories.Models.UsersDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<IEnumerable<ViewUser>> GetAllAsync();
        Task<ViewUser> GetByIdAsync(Guid id);
        Task<ViewUser> CreateAsync(CreateUser dto);
        Task<ViewUser> UpdateAsync(Guid id, UpdateUser dto);
        Task<bool> DeleteAsync(Guid id);
        Task<ViewUserShortInfo> GetUserShortInfoAsync(Guid userId);
        Task<IEnumerable<ViewUser>> GetByDeptIdAsync(int deptId);
        Task<Guid?> GetDirectorIdAsync();
        Task<Guid?> GetAuditeeOwnerByDepartmentIdAsync(int deptId);
        Task<ViewUserShortInfo?> GetAuditeeOwnerInfoByDepartmentIdAsync(int deptId);
        Task<bool> UserExistsAsync(Guid userId);
        Task<Guid?> GetLeadAuditorIdAsync();
        Task<IEnumerable<Entities.UserAccount>> GetUsersByRolesAsync(string[] roleNames);
    }
}
