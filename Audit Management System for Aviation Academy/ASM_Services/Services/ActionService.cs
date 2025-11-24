using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ActionDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ASM_Services.Services
{
    public class ActionService : IActionService
    {
        private readonly IActionRepository _repo;
        private readonly IUsersRepository _userRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IAttachmentRepository _attachmentRepo;
        private readonly IFindingRepository _findingRepo;
        private readonly IAuditTeamRepository _auditTeamRepo;
        public ActionService(IActionRepository repo, IUsersRepository userRepo, INotificationRepository notificationRepo, IAttachmentRepository attachmentRepo, IFindingRepository findingRepo, IAuditTeamRepository auditTeamRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _notificationRepo = notificationRepo;
            _attachmentRepo = attachmentRepo;
            _findingRepo = findingRepo;
            _auditTeamRepo = auditTeamRepo;
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
        public Task<bool> UpdateStatusToApprovedAuditorAsync(Guid id) => _repo.UpdateStatusToApprovedAuditorAsync(id);
        public Task<IEnumerable<ViewAction>> GetByFindingIdAsync(Guid findingId) => _repo.GetByFindingIdAsync(findingId);

        public async Task<bool> ActionVerifiedAsync(Guid id, string reviewFeedback)
        {
            return await _repo.UpdateStatusToVerifiedAsync(id, reviewFeedback);
        }
        public async Task<Notification> ActionDeclinedAsync(Guid actionId, Guid userBy, string reviewFeedback)
        {
            await _repo.UpdateStatusToDeclinedAsync(actionId, reviewFeedback);

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            var user = await _userRepo.GetUserShortInfoAsync(userBy);
            if (user == null)
                throw new Exception("User not found");

            if (action.AssignedTo == null)
                throw new Exception("AssignedTo is null");

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = action.AssignedTo.Value,
                Title = "Your action has been declined by AuditeeOwner",
                Message = $"Your action '{action.Title}' has been declined by {user.FullName} ({user.RoleName})." +
                        (string.IsNullOrWhiteSpace(reviewFeedback) ? "" : $"\nFeedback: {reviewFeedback}"),
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
        }

        public async Task<List<Notification>> ActionApprovedAsync(Guid actionId, Guid userBy, string reviewFeedback)
        {
            await _repo.UpdateStatusToApprovedAsync(actionId, reviewFeedback);

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            var user = await _userRepo.GetUserShortInfoAsync(userBy);
            if (user == null)
                throw new Exception("User not found");

            if (action.AssignedBy == null)
                throw new Exception("AssignedBy is null");

            var notif1 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = action.AssignedBy.Value,
                Title = "Your action has been approved by Auditor",
                Message = $"Your action '{action.Title}' has been approved by {user.FullName} ({user.RoleName})." +
                        (!string.IsNullOrEmpty(reviewFeedback) ? $"\nFeedback: {reviewFeedback}" : ""),
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });

            var findingId = await _repo.GetFindingIdByActionIdAsync(actionId);
            if (findingId == null)
                throw new Exception("FindingId not found for this Action");

            var auditId = await _findingRepo.GetAuditIdByFindingIdAsync(findingId.Value);
            if (auditId == null)
                throw new Exception("AuditId not found for this Finding");

            var leadId = await _auditTeamRepo.GetLeadUserIdByAuditIdAsync(auditId.Value);
            if (leadId == null)
                throw new Exception("LeadId not found for this Audit");

            var notif2 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = leadId.Value,
                Title = "Action approved by Auditor – needs your review",
                Message = $"Auditor {user.FullName} ({user.RoleName}) has approved the action '{action.Title}'.\n" +
                        "Please review and provide your feedback or approval as Lead Auditor.",
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });

            return new List<Notification> { notif1, notif2 };
        }



        public async Task<Notification> ActionRejectedAsync(Guid actionId, Guid userBy, string reviewFeedback)
        {
            await _repo.UpdateStatusToReturnedAsync(actionId, reviewFeedback);

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            var user = await _userRepo.GetUserShortInfoAsync(userBy);
            if (user == null)
                throw new Exception("User not found");

            if (action.AssignedBy == null)
                throw new Exception("AssignedBy is null");

            var notif = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = action.AssignedBy.Value,
                Title = "Your action was rejected by Auditor",
                Message = $"Your action '{action.Title}' has been rejected by {user.FullName} ({user.RoleName})." +
                        (!string.IsNullOrEmpty(reviewFeedback) ? $"\nFeedback: {reviewFeedback}" : ""),
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });

            return notif;
        }

        public async Task<List<Notification>> ApproveByHigherLevel(Guid actionId, Guid userBy, string reviewFeedback)
        {
            var attachmentId = await _attachmentRepo.GetAttachmentIdsByActionIdAsync(actionId);
            if (attachmentId == null || !attachmentId.Any())
                throw new InvalidOperationException($"Attachment not found for ActionId = {actionId}");

            var findingId = await _repo.GetFindingIdByActionIdAsync(actionId);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId}");

            await _repo.UpdateStatusToCompletedAsync(actionId, reviewFeedback);
            await _attachmentRepo.UpdateStatusAsync(attachmentId, "Completed");
            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Closed");

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            var user = await _userRepo.GetUserShortInfoAsync(userBy);
            if (user == null)
                throw new Exception("User not found");

            if (action.AssignedBy == null)
                throw new Exception("AssignedBy is null");

            var notif1 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = action.AssignedBy.Value,
                Title = "Your action has been completed by Lead Auditor",
                Message = $"Your action '{action.Title}' has been approved by {user.FullName} ({user.RoleName})." +
                        (!string.IsNullOrEmpty(reviewFeedback) ? $"\nLead Feedback: {reviewFeedback}" : "") +
                        "\nThe action and attachment is now marked as Completed",
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });

            var createdById = await _findingRepo.GetCreatedByIdByFindingIdAsync(findingId.Value);
            if (!createdById.HasValue)
                throw new Exception("CreatedById not found for this Finding");

            var notif2 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = createdById.Value,
                Title = "Finding has been closed by Lead Auditor",
                Message = $"The finding associated with action '{action.Title}' has been approved by {user.FullName} ({user.RoleName})." +
                        (!string.IsNullOrEmpty(reviewFeedback) ? $"\nLead Feedback: {reviewFeedback}" : "") +
                        "\nThe finding is now marked as Closed.",
                EntityType = "Finding",
                EntityId = findingId,
                IsRead = false,
                Status = "Active",
            });

            return new List<Notification> { notif1, notif2 };
        }

        public async Task<List<Notification>> RejectByHigherLevel(Guid actionId, Guid userBy, string reviewFeedback)
        {
            var attachmentId = await _attachmentRepo.GetAttachmentIdsByActionIdAsync(actionId);
            if (attachmentId == null || !attachmentId.Any())
                throw new InvalidOperationException($"Attachment not found for ActionId = {actionId}");

            var findingId = await _repo.GetFindingIdByActionIdAsync(actionId);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId}");

            await _repo.UpdateStatusToRejectedAsync(actionId, reviewFeedback);
            await _attachmentRepo.UpdateStatusAsync(attachmentId, "Rejected");
            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Reopen");

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            var user = await _userRepo.GetUserShortInfoAsync(userBy);
            if (user == null)
                throw new Exception("User not found");

            if (action.AssignedBy == null)
                throw new Exception("AssignedBy is null");

            var notif1 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = action.AssignedBy.Value,
                Title = "Your action has been rejected by Lead Auditor",
                Message = $"Your action '{action.Title}' has been rejected by {user.FullName} ({user.RoleName})." +
                        (!string.IsNullOrEmpty(reviewFeedback) ? $"\nLead Feedback: {reviewFeedback}" : "") +
                        "\nThe action and attachment is now marked as rejected",
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });

            var createdById = await _findingRepo.GetCreatedByIdByFindingIdAsync(findingId.Value);
            if (!createdById.HasValue)
                throw new Exception("CreatedById not found for this Finding");

            var notif2 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = createdById.Value,
                Title = "Finding has been reopen by Lead Auditor",
                Message = $"The finding associated with action '{action.Title}' has been reopen by {user.FullName} ({user.RoleName})." +
                        (!string.IsNullOrEmpty(reviewFeedback) ? $"\nLead Feedback: {reviewFeedback}" : "") +
                        "\nThe finding is now marked as reopen.",
                EntityType = "Finding",
                EntityId = findingId,
                IsRead = false,
                Status = "Active",
            });

            return new List<Notification> { notif1, notif2 };
        }

    }

}
                                                                                                                                                                                                                            