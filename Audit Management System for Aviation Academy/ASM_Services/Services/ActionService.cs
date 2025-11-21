using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ActionDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class ActionService : IActionService
    {
        private readonly IActionRepository _repo;
        private readonly IUsersRepository _userRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IAttachmentRepository _attachmentRepo;
        private readonly IFindingRepository _findingRepo;
        public ActionService(IActionRepository repo, IUsersRepository userRepo, INotificationRepository notificationRepo, IAttachmentRepository attachmentRepo, IFindingRepository findingRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _notificationRepo = notificationRepo;
            _attachmentRepo = attachmentRepo;
            _findingRepo = findingRepo;
        }

        public Task<IEnumerable<ViewAction>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAction?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewAction> CreateAsync(CreateAction dto) => _repo.CreateAsync(dto);
        public Task<ViewAction> UpdateAsync(Guid id, UpdateAction dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> DeleteAsync(Guid id) => _repo.SoftDeleteAsync(id);
        public Task<bool> UpdateStatusToInProgressAsync(Guid id) => _repo.UpdateStatusToInProgressAsync(id);
        public Task<bool> UpdateStatusToReviewedAsync(Guid id) => _repo.UpdateStatusToReviewedAsync(id);
        public Task<bool> UpdateStatusToApprovedAsync(Guid id) => _repo.UpdateStatusToApprovedAsync(id);
        public Task<bool> UpdateStatusToRejectedAsync(Guid id) => _repo.UpdateStatusToRejectedAsync(id);
        public Task<bool> UpdateStatusToClosedAsync(Guid id) => _repo.UpdateStatusToClosedAsync(id);
        public Task<IEnumerable<ViewAction>> GetByAssignedToAsync(Guid userId) => _repo.GetByAssignedToAsync(userId);
        public Task<bool> UpdateProgressPercentAsync(Guid id, byte progressPercent) => _repo.UpdateProgressPercentAsync(id, progressPercent);

        public async Task<Notification> ActionApprovedAsync(Guid actionId, Guid rejectedBy, string reviewFeedback)
        {
            await _repo.UpdateStatusToApprovedAsync(actionId, reviewFeedback);

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            var user = await _userRepo.GetUserShortInfoAsync(rejectedBy);
            if (user == null)
                throw new Exception("User not found");

            if (action.AssignedBy == null)
                throw new Exception("AssignedBy is null");

            Guid ownerId = action.AssignedBy.Value;

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = ownerId,
                Title = "Your attachment was rejected",
                Message = $"Title {action.Title} has been rejected.\n" +
                          $"Reason: {reviewFeedback}.\n" +
                          $"Rejected by: {user.FullName}.\n" +
                          $"Role name: {user.RoleName}",
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
        }



        public async Task<Notification> ActionRejectedAsync(Guid actionId, Guid rejectedBy, string reviewFeedback)
        {
            await _repo.UpdateStatusToReturnedAsync(actionId, reviewFeedback);

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            var user = await _userRepo.GetUserShortInfoAsync(rejectedBy);
            if (user == null)
                throw new Exception("User not found");

            if (action.AssignedBy == null)
                throw new Exception("AssignedBy is null");

            Guid ownerId = action.AssignedBy.Value;

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = ownerId,
                Title = "Your attachment was rejected",
                Message = $"Title {action.Title} has been rejected.\n" +
                          $"Reason: {reviewFeedback}.\n" +
                          $"Rejected by: {user.FullName}.\n" +
                          $"Role name: {user.RoleName}",
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
        }

        public async Task ApproveByHigherLevel(Guid actionId, string reviewFeedback)
        {
            await _repo.UpdateStatusToCompletedAsync(actionId, reviewFeedback);

            var attachmentId = await _attachmentRepo.GetAttachmentIdsByActionIdAsync(actionId);
            if (attachmentId == null || !attachmentId.Any())
                throw new InvalidOperationException($"Attachment not found for ActionId = {actionId}");

            await _attachmentRepo.UpdateStatusAsync(attachmentId, "Completed");

            var findingId = await _repo.GetFindingIdByActionIdAsync(actionId);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId}");

            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Closed");
        }

        public async Task RejectByHigherLevel(Guid actionId, string reviewFeedback)
        {
            await _repo.UpdateStatusToRejectedAsync(actionId, reviewFeedback);

            var attachmentId = await _attachmentRepo.GetAttachmentIdsByActionIdAsync(actionId);
            if (attachmentId == null || !attachmentId.Any())
                throw new InvalidOperationException($"Attachment not found for ActionId = {actionId}");

            await _attachmentRepo.UpdateStatusAsync(attachmentId, "Rejected");

            var findingId = await _repo.GetFindingIdByActionIdAsync(actionId);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId}");

            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Reopen");
        }

    }

}
                                                                                                                                                                                                                            