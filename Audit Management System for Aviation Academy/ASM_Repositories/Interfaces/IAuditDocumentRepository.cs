using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditCriterionDTO;
using ASM_Repositories.Models.AuditDocumentDTO;
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
        Task<AuditDocument?> UpdateAsync(Guid auditId, Action<AuditDocument> updateAction);
        Task<ViewAuditDocument?> GetAuditDocumentByAuditIdAsync(Guid auditId);
        Task AddAsync(AuditDocument doc);
    }
}
