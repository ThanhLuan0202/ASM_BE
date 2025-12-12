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
        Task<ViewAuditScopeDepartment> CreateAsync(CreateAuditScopeDepartment dto, Guid userId);
        Task<ViewAuditScopeDepartment?> UpdateAsync(Guid id, UpdateAuditScopeDepartment dto, Guid userId);
        Task<bool> SoftDeleteAsync(Guid id, Guid userId);
        Task<IEnumerable<ViewDepartment>> GetDepartmentsByAuditIdAsync(Guid auditId);
        Task<SensitiveFlagResponse> SetSensitiveFlagAsync(Guid scopeDeptId, SetSensitiveFlagRequest request, Guid userId);
        Task<IEnumerable<SensitiveFlagResponse>> GetSensitiveFlagsByAuditIdAsync(Guid auditId);
    }
}
