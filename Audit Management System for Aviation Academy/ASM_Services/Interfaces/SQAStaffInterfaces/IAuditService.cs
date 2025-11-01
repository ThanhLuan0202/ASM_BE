using ASM_Repositories.Models.AuditDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IAuditService
    {
        Task<IEnumerable<ViewAudit>> GetAllAuditAsync();
        Task<ViewAudit?> GetAuditByIdAsync(Guid id);
        Task<ViewAudit> CreateAuditAsync(CreateAudit dto);
        Task<ViewAudit?> UpdateAuditAsync(Guid id, UpdateAudit dto);
        Task<bool> DeleteAuditAsync(Guid id);
    }
}
