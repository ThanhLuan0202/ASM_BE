using ASM_Repositories.Models;
using ASM_Repositories.Models.DepartmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<ViewDepartment>> GetAllDepartmentsAsync();
        Task<ViewDepartment?> GetDepartmentByIdAsync(int id);
        Task<ViewDepartment> CreateDepartmentAsync(CreateDepartment dto);
        Task<ViewDepartment?> UpdateDepartmentAsync(int id, UpdateDepartment dto);
        Task<bool> DeleteDepartmentAsync(int id);
    }
}
