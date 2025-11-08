using ASM_Repositories.Models.DepartmentHeadDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.AdminInterfaces
{
    public interface IDepartmentHeadRepository
    {
        Task<IEnumerable<ViewDepartmentHead>> GetAllAsync();
        Task<ViewDepartmentHead?> GetByIdAsync(Guid deptHeadId);
        Task<ViewDepartmentHead> CreateAsync(CreateDepartmentHead dto);
        Task<ViewDepartmentHead?> UpdateAsync(Guid deptHeadId, UpdateDepartmentHead dto);
        Task<bool> DeleteAsync(Guid deptHeadId);
    }
}

