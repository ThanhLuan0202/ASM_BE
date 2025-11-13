using ASM_Repositories.Models.AttachmentDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.AdminInterfaces
{
    public interface IAttachmentService
    {
        Task<IEnumerable<ViewAttachment>> GetAllAsync();
        Task<ViewAttachment?> GetByIdAsync(Guid attachmentId);
        Task<IEnumerable<ViewAttachment>> GetByEntityAsync(string entityType, Guid entityId);
        Task<ViewAttachment> CreateAsync(CreateAttachment dto, IFormFile file, Guid? uploadedBy);
        Task<ViewAttachment?> UpdateAsync(Guid attachmentId, UpdateAttachment dto);
        Task<ViewAttachment?> UpdateFileAsync(Guid attachmentId, IFormFile file);
        Task<bool> DeleteAsync(Guid attachmentId);
    }
}

