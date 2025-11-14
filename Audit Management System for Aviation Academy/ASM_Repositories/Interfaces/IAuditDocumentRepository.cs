using ASM_Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditDocumentRepository
    {
        Task AddAuditDocumentAsync(AuditDocument doc);
        Task<AuditDocument?> UpdateStatusByAuditIdAsync(Guid auditId, string status);
    }
}
