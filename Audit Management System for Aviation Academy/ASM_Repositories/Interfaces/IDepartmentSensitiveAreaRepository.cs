using ASM_Repositories.Models.DepartmentSensitiveAreaDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IDepartmentSensitiveAreaRepository
    {
        Task<IEnumerable<ViewDepartmentSensitiveArea>> GetAllAsync();
        Task<ViewDepartmentSensitiveArea?> GetByIdAsync(Guid id);
        Task<ViewDepartmentSensitiveArea?> GetByDeptIdAsync(int deptId);
        Task<ViewDepartmentSensitiveArea> CreateAsync(CreateDepartmentSensitiveArea dto, string createdBy);
        Task<ViewDepartmentSensitiveArea?> UpdateAsync(Guid id, UpdateDepartmentSensitiveArea dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsByDeptIdAsync(int deptId);
    }
}

