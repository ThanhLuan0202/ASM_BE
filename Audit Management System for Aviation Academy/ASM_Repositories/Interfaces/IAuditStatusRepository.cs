using ASM_Repositories.Models.AuditStatusDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditStatusRepository
    {
        Task<IEnumerable<ViewAuditStatus>> GetAllAsync();
        Task<ViewAuditStatus?> GetByIdAsync(string auditStatus);
        Task<ViewAuditStatus> CreateAsync(CreateAuditStatus dto);
        Task<ViewAuditStatus?> UpdateAsync(string auditStatus, UpdateAuditStatus dto);
        Task<bool> DeleteAsync(string auditStatus);
    }
}

