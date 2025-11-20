using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AttachmentDTO;
using ASM_Repositories.Models.NotificationDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAttachmentRepository _repo;
        private readonly IFirebaseUploadService _firebaseUploadService;
        private readonly IActionRepository _actionRepo;
        private readonly IFindingRepository _findingRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IUsersRepository _userRepo;

        public AttachmentService(IAttachmentRepository repo, IFirebaseUploadService firebaseUploadService, IActionRepository actionRepo, IFindingRepository findingRepo, INotificationRepository notificationRepo, IUsersRepository userRepo)
        {
            _repo = repo;
            _firebaseUploadService = firebaseUploadService;
            _actionRepo = actionRepo;
            _findingRepo = findingRepo;
            _notificationRepo = notificationRepo;
            _userRepo = userRepo;
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

        public async Task<List<Attachment>> GetAttachmentsAsync(List<Guid> findingIds) => await _repo.GetAttachmentsAsync(findingIds);

        public async Task UpdateAttachmentStatusAsync(Guid attachmentId, string status) =>  await _repo.UpdateAttachmentStatusAsync(attachmentId, status);

        public async Task ApproveByHigherLevel(Guid attachmentId)
        {
            await _repo.UpdateAttachmentStatusAsync(attachmentId, "Completed");

            var actionId = await _repo.GetEntityIdByAttachmentIdAsync(attachmentId);
            if (!actionId.HasValue)
                throw new InvalidOperationException($"Action not found for AttachmentId = {attachmentId}");

            await _actionRepo.UpdateActionStatusAsync(actionId.Value, "Completed");

            var findingId = await _actionRepo.GetFindingIdByActionIdAsync(actionId.Value);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId.Value}");

            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Closed");
        }

        public async Task RejectByHigherLevel(Guid attachmentId)
        {
            await _repo.UpdateAttachmentStatusAsync(attachmentId, "Rejected");

            var actionId = await _repo.GetEntityIdByAttachmentIdAsync(attachmentId);
            if (!actionId.HasValue)
                throw new InvalidOperationException($"Action not found for AttachmentId = {attachmentId}");

            await _actionRepo.UpdateActionStatusAsync(actionId.Value, "Rejected");

            var findingId = await _actionRepo.GetFindingIdByActionIdAsync(actionId.Value);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId.Value}");

            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Reopen");
        }

        public async Task RejectAttachmentAsync(Guid attachmentId, Guid rejectedBy, string reason)
        {

            await _repo.RejectAttachmentAsync(attachmentId);

            var attachment = await _repo.GetByIdAsync(attachmentId);   
            
            var user = await _userRepo.GetUserShortInfoAsync(rejectedBy);

            Guid? ownerId = attachment.UploadedBy.Value;

            await _notificationRepo.CreateAsync(new CreateNotification
            {
                UserId = ownerId.Value,
                Title = "Your attachment was rejected",
                Message = $"File {attachment.FileName} has been rejected.\n" +
                            $"Reason: {reason}.\n" +
                            $"Rejected by: {user.FullName}.\n" +
                            $"Role name: {user.RoleName}",
                EntityType = attachment.EntityType,
                EntityId = attachment.AttachmentId,
                IsRead = false,
            });
        }

        public async Task<Notification> AttachmentRejectedAsync(Guid attachmentId, Guid rejectedBy, string reason)
        {
            await _repo.RejectAttachmentAsync(attachmentId);

            var attachment = await _repo.GetByIdAsync(attachmentId);
            var user = await _userRepo.GetUserShortInfoAsync(rejectedBy);

            Guid ownerId = attachment.UploadedBy.Value;

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = ownerId,
                Title = "Your attachment was rejected",
                Message = $"File {attachment.FileName} has been rejected.\n" +
                          $"Reason: {reason}.\n" +
                          $"Rejected by: {user.FullName}.\n" +
                          $"Role name: {user.RoleName}",
                EntityType = attachment.EntityType,
                EntityId = attachment.AttachmentId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
        }


    }
}

