using ASM_Repositories.Entities;
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
        Task<List<Attachment>> GetAttachmentsAsync(List<Guid> findingIds);
        Task UpdateAttachmentStatusAsync(Guid attachmentId, string status);
        Task ApproveByHigherLevel(Guid attachmentId);
        Task RejectByHigherLevel(Guid attachmentId);
        Task RejectAttachmentAsync(Guid attachmentId, Guid rejectedBy, string reason);
    }
}

