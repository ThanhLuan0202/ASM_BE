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
        Task<ViewAttachment?> UpdateAsync(Guid attachmentId, UpdateAttachment dto, Guid userId);
        Task<ViewAttachment?> UpdateFileAsync(Guid attachmentId, IFormFile file, Guid userId);
        Task<bool> DeleteAsync(Guid attachmentId, Guid userId);
        Task<List<Attachment>> GetAttachmentsAsync(List<Guid> findingIds);
        Task UpdateAttachmentStatusAsync(Guid attachmentId, string status, Guid userId);
        Task ApproveByHigherLevel(Guid attachmentId, Guid userId);
        Task RejectByHigherLevel(Guid attachmentId, Guid userId);
        Task RejectAttachmentAsync(Guid attachmentId, Guid rejectedBy, string reason);
        Task<Notification> AttachmentRejectedAsync(Guid attachmentId, Guid rejectedBy, string reason);
    }
}

