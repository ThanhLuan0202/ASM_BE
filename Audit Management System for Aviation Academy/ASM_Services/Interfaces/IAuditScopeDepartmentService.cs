using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using ASM_Repositories.Models.DepartmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DepartmentInfoDto = ASM_Repositories.Interfaces.DepartmentInfoDto;

namespace ASM_Services.Interfaces
{
    public interface IAuditScopeDepartmentService
    {
        Task<IEnumerable<ViewAuditScopeDepartment>> GetAllAsync();
        Task<ViewAuditScopeDepartment?> GetByIdAsync(Guid id);
        Task<ViewAuditScopeDepartment> CreateAsync(CreateAuditScopeDepartment dto);
        Task<ViewAuditScopeDepartment?> UpdateAsync(Guid id, UpdateAuditScopeDepartment dto);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<IEnumerable<ViewDepartment>> GetDepartmentsByAuditIdAsync(Guid auditId);
    }
}
