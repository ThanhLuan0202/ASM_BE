using ASM_Repositories.Models.DepartmentSensitiveAreaDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IDepartmentSensitiveAreaService
    {
        Task<IEnumerable<ViewDepartmentSensitiveArea>> GetAllAsync();
        Task<ViewDepartmentSensitiveArea?> GetByIdAsync(Guid id);
        Task<ViewDepartmentSensitiveArea?> GetByDeptIdAsync(int deptId);
        Task<ViewDepartmentSensitiveArea> CreateAsync(CreateDepartmentSensitiveArea dto, Guid userId);
        Task<ViewDepartmentSensitiveArea?> UpdateAsync(Guid id, UpdateDepartmentSensitiveArea dto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}

