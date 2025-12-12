using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using ASM_Repositories.Models.DepartmentDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditScopeDepartmentRepository
    {
        Task<IEnumerable<ViewAuditScopeDepartment>> GetAllAsync();
        Task<ViewAuditScopeDepartment?> GetByIdAsync(Guid id);
        Task<ViewAuditScopeDepartment> AddAsync(CreateAuditScopeDepartment dto);
        Task<ViewAuditScopeDepartment?> UpdateAsync(Guid id, UpdateAuditScopeDepartment dto);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<List<AuditScopeDepartment>> GetAuditScopeDepartmentsAsync(Guid auditId);
        Task UpdateScopeDepartmentsAsync(Guid auditId, List<UpdateAuditScopeDepartment>? list);
        Task<IEnumerable<ViewDepartment>> GetDepartmentsByAuditIdAsync(Guid auditId);
        Task UpdateStatusToArchivedAsync(Guid auditId);
        Task<SensitiveFlagResponse> SetSensitiveFlagAsync(Guid scopeDeptId, SetSensitiveFlagRequest request);
        Task<IEnumerable<SensitiveFlagResponse>> GetSensitiveFlagsByAuditIdAsync(Guid auditId);
    }

    public class DepartmentInfoDto
    {
        public int DeptId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
