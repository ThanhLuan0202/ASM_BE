using ASM_Repositories.Models.AuditApprovalDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IAuditApprovalService
    {
        Task<IEnumerable<ViewAuditApproval>> GetAllAsync();
        Task<ViewAuditApproval?> GetByIdAsync(Guid id);
        Task<ViewAuditApproval> CreateAsync(CreateAuditApproval dto);
        Task<ViewAuditApproval> UpdateAsync(Guid id, UpdateAuditApproval dto);
        Task<bool> SoftDeleteAsync(Guid id, Guid userId);
    }
}
