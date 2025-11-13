using ASM_Repositories.Models.AttachmentDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAttachmentRepository
    {
        Task<IEnumerable<ViewAttachment>> GetAllAsync();
        Task<ViewAttachment?> GetByIdAsync(Guid attachmentId);
        Task<IEnumerable<ViewAttachment>> GetByEntityAsync(string entityType, Guid entityId);
        Task<ViewAttachment> CreateAsync(CreateAttachment dto, string fileName, string blobPath, string contentType, long sizeBytes, Guid? uploadedBy);
        Task<ViewAttachment?> UpdateAsync(Guid attachmentId, UpdateAttachment dto);
        Task<ViewAttachment?> UpdateFileAsync(Guid attachmentId, string fileName, string blobPath, string contentType, long sizeBytes);
        Task<bool> DeleteAsync(Guid attachmentId);
    }
}

