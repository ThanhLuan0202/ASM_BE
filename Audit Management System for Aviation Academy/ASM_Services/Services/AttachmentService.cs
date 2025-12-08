using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AttachmentDTO;
using ASM_Repositories.Models.NotificationDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces;
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
        private readonly IAuditLogService _logService;

        public AttachmentService(IAttachmentRepository repo, IFirebaseUploadService firebaseUploadService, IActionRepository actionRepo, IFindingRepository findingRepo, INotificationRepository notificationRepo, IUsersRepository userRepo, IAuditLogService logService)
        {
            _repo = repo;
            _firebaseUploadService = firebaseUploadService;
            _actionRepo = actionRepo;
            _findingRepo = findingRepo;
            _notificationRepo = notificationRepo;
            _userRepo = userRepo;
            _logService = logService;
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
            var created = await _repo.CreateAsync(
                dto,
                file.FileName,
                blobPath,
                file.ContentType,
                file.Length,
                uploadedBy
            );

            if (uploadedBy.HasValue)
            {
                await _logService.LogCreateAsync(created, created.AttachmentId, uploadedBy.Value, "Attachment");
            }

            return created;
        }

        public async Task<ViewAttachment?> UpdateAsync(Guid attachmentId, UpdateAttachment dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(attachmentId);
            var updated = await _repo.UpdateAsync(attachmentId, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, attachmentId, userId, "Attachment");
            }
            return updated;
        }

        public async Task<ViewAttachment?> UpdateFileAsync(Guid attachmentId, IFormFile file, Guid userId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is required and cannot be empty");

            // Upload new file to Firebase
            var blobPath = await _firebaseUploadService.UploadFileAsync(file, "Attachments");

            // Update attachment record with new file info
            var before = await _repo.GetByIdAsync(attachmentId);
            var updated = await _repo.UpdateFileAsync(
                attachmentId,
                file.FileName,
                blobPath,
                file.ContentType,
                file.Length
            );

            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, attachmentId, userId, "Attachment");
            }

            return updated;
        }

        public async Task<bool> DeleteAsync(Guid attachmentId, Guid userId)
        {
            var before = await _repo.GetByIdAsync(attachmentId);
            var success = await _repo.DeleteAsync(attachmentId);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, attachmentId, userId, "Attachment");
            }
            return success;
        }

        public async Task<List<Attachment>> GetAttachmentsAsync(List<Guid> findingIds) => await _repo.GetAttachmentsAsync(findingIds);

        public async Task UpdateAttachmentStatusAsync(Guid attachmentId, string status, Guid userId)
        {
            var before = await _repo.GetByIdAsync(attachmentId);
            await _repo.UpdateAttachmentStatusAsync(attachmentId, status);
            var after = await _repo.GetByIdAsync(attachmentId);
            if (before != null && after != null)
            {
                await _logService.LogUpdateAsync(before, after, attachmentId, userId, "Attachment");
            }
        }

        public async Task ApproveByHigherLevel(Guid attachmentId, Guid userId)
        {
            var beforeAttachment = await _repo.GetByIdAsync(attachmentId);
            await _repo.UpdateAttachmentStatusAsync(attachmentId, "Completed");

            var actionId = await _repo.GetEntityIdByAttachmentIdAsync(attachmentId);
            if (!actionId.HasValue)
                throw new InvalidOperationException($"Action not found for AttachmentId = {attachmentId}");

            var beforeAction = await _actionRepo.GetByIdAsync(actionId.Value);
            await _actionRepo.UpdateActionStatusAsync(actionId.Value, "Completed");

            var findingId = await _actionRepo.GetFindingIdByActionIdAsync(actionId.Value);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId.Value}");

            var beforeFinding = await _findingRepo.GetFindingByIdAsync(findingId.Value);
            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Closed");

            // After states
            var afterAttachment = await _repo.GetByIdAsync(attachmentId);
            if (beforeAttachment != null && afterAttachment != null)
                await _logService.LogUpdateAsync(beforeAttachment, afterAttachment, attachmentId, userId, "Attachment");

            if (beforeAction != null)
            {
                var afterAction = await _actionRepo.GetByIdAsync(actionId.Value);
                if (afterAction != null)
                    await _logService.LogUpdateAsync(beforeAction, afterAction, actionId.Value, userId, "Action");
            }

            if (beforeFinding != null)
            {
                var afterFinding = await _findingRepo.GetFindingByIdAsync(findingId.Value);
                if (afterFinding != null)
                    await _logService.LogUpdateAsync(beforeFinding, afterFinding, findingId.Value, userId, "Finding");
            }
        }

        public async Task RejectByHigherLevel(Guid attachmentId, Guid userId)
        {
            var beforeAttachment = await _repo.GetByIdAsync(attachmentId);
            await _repo.UpdateAttachmentStatusAsync(attachmentId, "Rejected");

            var actionId = await _repo.GetEntityIdByAttachmentIdAsync(attachmentId);
            if (!actionId.HasValue)
                throw new InvalidOperationException($"Action not found for AttachmentId = {attachmentId}");

            var beforeAction = await _actionRepo.GetByIdAsync(actionId.Value);
            await _actionRepo.UpdateActionStatusAsync(actionId.Value, "Rejected");

            var findingId = await _actionRepo.GetFindingIdByActionIdAsync(actionId.Value);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId.Value}");

            var beforeFinding = await _findingRepo.GetFindingByIdAsync(findingId.Value);
            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Reopen");

            // After states
            var afterAttachment = await _repo.GetByIdAsync(attachmentId);
            if (beforeAttachment != null && afterAttachment != null)
                await _logService.LogUpdateAsync(beforeAttachment, afterAttachment, attachmentId, userId, "Attachment");

            if (beforeAction != null)
            {
                var afterAction = await _actionRepo.GetByIdAsync(actionId.Value);
                if (afterAction != null)
                    await _logService.LogUpdateAsync(beforeAction, afterAction, actionId.Value, userId, "Action");
            }

            if (beforeFinding != null)
            {
                var afterFinding = await _findingRepo.GetFindingByIdAsync(findingId.Value);
                if (afterFinding != null)
                    await _logService.LogUpdateAsync(beforeFinding, afterFinding, findingId.Value, userId, "Finding");
            }
        }

        public async Task RejectAttachmentAsync(Guid attachmentId, Guid rejectedBy, string reason)
        {
            var before = await _repo.GetByIdAsync(attachmentId);
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

            if (before != null && attachment != null)
            {
                await _logService.LogUpdateAsync(before, attachment, attachmentId, rejectedBy, "Attachment");
            }
        }

        public async Task<Notification> AttachmentRejectedAsync(Guid attachmentId, Guid rejectedBy, string reason)
        {
            var before = await _repo.GetByIdAsync(attachmentId);
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

            if (before != null && attachment != null)
            {
                await _logService.LogUpdateAsync(before, attachment, attachmentId, rejectedBy, "Attachment");
            }

            return notif;
        }


    }
}

