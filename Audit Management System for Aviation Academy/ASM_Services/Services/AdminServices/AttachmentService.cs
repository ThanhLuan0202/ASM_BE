using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.AttachmentDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services.AdminServices
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _repo;
        private readonly IFirebaseUploadService _firebaseUploadService;

        public AttachmentService(IAttachmentRepository repo, IFirebaseUploadService firebaseUploadService)
        {
            _repo = repo;
            _firebaseUploadService = firebaseUploadService;
        }

        public Task<IEnumerable<ViewAttachment>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAttachment?> GetByIdAsync(Guid attachmentId) => _repo.GetByIdAsync(attachmentId);
        public Task<IEnumerable<ViewAttachment>> GetByEntityAsync(string entityType, Guid entityId) => _repo.GetByEntityAsync(entityType, entityId);

        public async Task<ViewAttachment> CreateAsync(CreateAttachment dto, IFormFile file, Guid? uploadedBy)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required and cannot be empty");

            // Upload file to Firebase
            var blobPath = await _firebaseUploadService.UploadFileAsync(file, "Attachments");

            // Create attachment record
            return await _repo.CreateAsync(
                dto,
                file.FileName,
                blobPath,
                file.ContentType,
                file.Length,
                uploadedBy
            );
        }

        public Task<ViewAttachment?> UpdateAsync(Guid attachmentId, UpdateAttachment dto) => _repo.UpdateAsync(attachmentId, dto);

        public async Task<ViewAttachment?> UpdateFileAsync(Guid attachmentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required and cannot be empty");

            // Upload new file to Firebase
            var blobPath = await _firebaseUploadService.UploadFileAsync(file, "Attachments");

            // Update attachment record with new file info
            return await _repo.UpdateFileAsync(
                attachmentId,
                file.FileName,
                blobPath,
                file.ContentType,
                file.Length
            );
        }

        public Task<bool> DeleteAsync(Guid attachmentId) => _repo.DeleteAsync(attachmentId);
    }
}

