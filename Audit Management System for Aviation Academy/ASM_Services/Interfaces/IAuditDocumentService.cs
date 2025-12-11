using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditDocumentDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IAuditDocumentService
    {
        Task<List<ViewAuditDocument?>> GetAuditDocumentByAuditIdAsync(Guid auditId);
        Task<AuditDocument?> UploadAndUpdateAuditDocumentAsync(Guid auditId, IFormFile file, Guid uploadedBy);
        Task<List<AuditDocument>> UploadMultipleAsync(Guid auditId, List<IFormFile> files, Guid uploadedBy, string documentType);
    }
}
