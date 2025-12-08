using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ActionDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
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
        private readonly IAuditLogService _logService;
        public ActionService(IActionRepository repo, IUsersRepository userRepo, INotificationRepository notificationRepo, IAttachmentRepository attachmentRepo, IFindingRepository findingRepo, IAuditTeamRepository auditTeamRepo, IAuditLogService auditLogService)
        {
            _repo = repo;
            _userRepo = userRepo;
            _notificationRepo = notificationRepo;
            _attachmentRepo = attachmentRepo;
            _findingRepo = findingRepo;
            _auditTeamRepo = auditTeamRepo;
            _logService = auditLogService;
        }

        public Task<IEnumerable<ViewAction>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAction?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public async Task<ViewAction> CreateAsync(CreateAction dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);

            await _logService.LogCreateAsync(created, created.ActionId, userId,"Action");

            return created;
        }
        public async Task<ViewAction> UpdateAsync(Guid id, UpdateAction dto, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateAsync(id, dto);

            if (updated != null && existing != null)
            {
                await _logService.LogUpdateAsync(existing, updated, id, userId, "Action");
            }

            return updated;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            var success = await _repo.SoftDeleteAsync(id);

            if (success && existing != null)
            {
                await _logService.LogSoftDeleteAsync(existing, existing, id, userId, "Action");
            }

            return success;
        }

        public async Task<bool> UpdateStatusToInProgressAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateStatusToInProgressAsync(id);
            if (updated && before != null)
            {
                var after = await _repo.GetByIdAsync(id);
                if (after != null)
                {
                    await _logService.LogUpdateAsync(before, after, id, userId, "Action");
                }
            }
            return updated;
        }

        public async Task<bool> UpdateStatusToReviewedAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateStatusToReviewedAsync(id);
            if (updated && before != null)
            {
                var after = await _repo.GetByIdAsync(id);
                if (after != null)
                {
                    await _logService.LogUpdateAsync(before, after, id, userId, "Action");
                }
            }
            return updated;
        }

        public async Task<bool> UpdateStatusToApprovedAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateStatusToApprovedAsync(id);
            if (updated && before != null)
            {
                var after = await _repo.GetByIdAsync(id);
                if (after != null)
                {
                    await _logService.LogUpdateAsync(before, after, id, userId, "Action");
                }
            }
            return updated;
        }
        
        public async Task<bool> UpdateStatusToRejectedAsync(Guid id, Guid userId)
        {
            // Update action status
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateStatusToRejectedAsync(id);
            if (!updated)
                return false;

            // Update attachments status
            try
            {
                var attachmentIds = await _attachmentRepo.GetAttachmentIdsByActionIdAsync(id);
                if (attachmentIds != null && attachmentIds.Any())
                {
                    await _attachmentRepo.UpdateStatusAsync(attachmentIds, "Rejected");
                }
            }
            catch (Exception)
            {
                // Log error nhưng không throw để không ảnh hưởng đến việc update action
                // Có thể log vào logger nếu cần
            }

            if (before != null)
            {
                var after = await _repo.GetByIdAsync(id);
                if (after != null)
                {
                    await _logService.LogUpdateAsync(before, after, id, userId, "Action");
                }
            }

            return true;
        }

        public async Task<bool> UpdateStatusToRejectedAsync(Guid id, string reviewFeedback, Guid userId)
        {
            // Update action status
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateStatusToRejectedAsync(id, reviewFeedback);
            if (!updated)
                return false;

            // Update attachments status
            try
            {
                var attachmentIds = await _attachmentRepo.GetAttachmentIdsByActionIdAsync(id);
                if (attachmentIds != null && attachmentIds.Any())
                {
                    await _attachmentRepo.UpdateStatusAsync(attachmentIds, "Rejected");
                }
            }
            catch (Exception)
            {
                // Log error nhưng không throw để không ảnh hưởng đến việc update action
                // Có thể log vào logger nếu cần
            }

            if (before != null)
            {
                var after = await _repo.GetByIdAsync(id);
                if (after != null)
                {
                    await _logService.LogUpdateAsync(before, after, id, userId, "Action");
                }
            }

            return true;
        }
        
        public async Task<bool> UpdateStatusToClosedAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateStatusToClosedAsync(id);
            if (updated && before != null)
            {
                var after = await _repo.GetByIdAsync(id);
                if (after != null)
                {
                    await _logService.LogUpdateAsync(before, after, id, userId, "Action");
                }
            }
            return updated;
        }
        public Task<IEnumerable<ViewAction>> GetByAssignedToAsync(Guid userId) => _repo.GetByAssignedToAsync(userId);
        public async Task<bool> UpdateProgressPercentAsync(Guid id, byte progressPercent, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateProgressPercentAsync(id, progressPercent);
            if (updated && before != null)
            {
                var after = await _repo.GetByIdAsync(id);
                if (after != null)
                {
                    await _logService.LogUpdateAsync(before, after, id, userId, "Action");
                }
            }
            return updated;
        }

        public async Task<bool> UpdateStatusToApprovedAuditorAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateStatusToApprovedAuditorAsync(id);
            if (updated && before != null)
            {
                var after = await _repo.GetByIdAsync(id);
                if (after != null)
                {
                    await _logService.LogUpdateAsync(before, after, id, userId, "Action");
                }
            }
            return updated;
        }
        public Task<IEnumerable<ViewAction>> GetByFindingIdAsync(Guid findingId) => _repo.GetByFindingIdAsync(findingId);
        public Task<IEnumerable<ViewAction>> GetByAssignedDeptIdAsync(int assignedDeptId) => _repo.GetByAssignedDeptIdAsync(assignedDeptId);

        public async Task<List<Notification>> ActionVerifiedAsync(Guid actionId, Guid userBy, string reviewFeedback)
        {
            var before = await _repo.GetByIdAsync(actionId);
            await _repo.UpdateStatusToVerifiedAsync(actionId, reviewFeedback);

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            if (before != null)
            {
                await _logService.LogUpdateAsync(before, action, actionId, userBy, "Action");
            }

            var user = await _userRepo.GetUserShortInfoAsync(userBy);
            if (user == null)
                throw new Exception("User not found");

            if (action.AssignedTo == null)
                throw new Exception("AssignedTo is null");

            var notif1 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = action.AssignedTo.Value,
                Title = "Your action has been verified by AuditeeOwner",
                Message = $"Your action '{action.Title}' has been verified by {user.FullName} ({user.RoleName})." +
                        (string.IsNullOrWhiteSpace(reviewFeedback) ? "" : $"\nFeedback: {reviewFeedback}"),
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });

            var findingId = await _repo.GetFindingIdByActionIdAsync(actionId);
            if (findingId == null)
                throw new Exception("FindingId not found for this Action");

            var finding = await _findingRepo.GetFindingByIdAsync(findingId.Value);

            var notif2 = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = finding.CreatedBy.Value,
                Title = "Your action has been verified by AuditeeOwner",
                Message = $"The action '{action.Title}' related to your finding has been verified by {user.FullName} ({user.RoleName})." +
                    (string.IsNullOrWhiteSpace(reviewFeedback) ? "" : $"\nReviewer feedback: {reviewFeedback}"),
                EntityType = "Action",
                EntityId = action.ActionId,
                IsRead = false,
                Status = "Active",
            });
            return new List<Notification> { notif1, notif2 };
        }
        public async Task<Notification> ActionDeclinedAsync(Guid actionId, Guid userBy, string reviewFeedback)
        {
            var before = await _repo.GetByIdAsync(actionId);
            await _repo.UpdateStatusToDeclinedAsync(actionId, reviewFeedback);

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            if (before != null)
            {
                await _logService.LogUpdateAsync(before, action, actionId, userBy, "Action");
            }

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
            var before = await _repo.GetByIdAsync(actionId);
            await _repo.UpdateStatusToApprovedAsync(actionId, reviewFeedback);

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            if (before != null)
            {
                await _logService.LogUpdateAsync(before, action, actionId, userBy, "Action");
            }

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

            var leadId = await _userRepo.GetLeadAuditorIdAsync();
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
            var before = await _repo.GetByIdAsync(actionId);
            await _repo.UpdateStatusToReturnedAsync(actionId, reviewFeedback);

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            if (before != null)
            {
                await _logService.LogUpdateAsync(before, action, actionId, userBy, "Action");
            }

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
            var before = await _repo.GetByIdAsync(actionId);
            var beforeAttachments = await _attachmentRepo.GetByEntityAsync("Action", actionId);

            var attachmentId = await _attachmentRepo.GetAttachmentIdsByActionIdAsync(actionId);
            if (attachmentId == null || !attachmentId.Any())
                throw new InvalidOperationException($"Attachment not found for ActionId = {actionId}");

            var findingId = await _repo.GetFindingIdByActionIdAsync(actionId);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId}");
            var beforeFinding = await _findingRepo.GetFindingByIdAsync(findingId.Value);

            await _repo.UpdateStatusToCompletedAsync(actionId, reviewFeedback);
            await _attachmentRepo.UpdateStatusAsync(attachmentId, "Completed");
            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Closed");

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            if (before != null)
            {
                await _logService.LogUpdateAsync(before, action, actionId, userBy, "Action");
            }
            var afterAttachments = await _attachmentRepo.GetByEntityAsync("Action", actionId);
            if (beforeAttachments != null && afterAttachments != null)
            {
                await _logService.LogUpdateAsync(beforeAttachments, afterAttachments, actionId, userBy, "Attachment");
            }
            var afterFinding = await _findingRepo.GetFindingByIdAsync(findingId.Value);
            if (beforeFinding != null && afterFinding != null)
            {
                await _logService.LogUpdateAsync(beforeFinding, afterFinding, findingId.Value, userBy, "Finding");
            }

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
            var before = await _repo.GetByIdAsync(actionId);
            var beforeAttachments = await _attachmentRepo.GetByEntityAsync("Action", actionId);

            var attachmentId = await _attachmentRepo.GetAttachmentIdsByActionIdAsync(actionId);
            if (attachmentId == null || !attachmentId.Any())
                throw new InvalidOperationException($"Attachment not found for ActionId = {actionId}");

            var findingId = await _repo.GetFindingIdByActionIdAsync(actionId);
            if (!findingId.HasValue)
                throw new InvalidOperationException($"Finding not found for ActionId = {actionId}");
            var beforeFinding = await _findingRepo.GetFindingByIdAsync(findingId.Value);

            await _repo.UpdateStatusToRejectedAsync(actionId, reviewFeedback);
            await _attachmentRepo.UpdateStatusAsync(attachmentId, "Rejected");
            await _findingRepo.UpdateFindingStatusAsync(findingId.Value, "Reopen");

            var action = await _repo.GetByIdAsync(actionId);
            if (action == null)
                throw new Exception("Action not found");

            if (before != null)
            {
                await _logService.LogUpdateAsync(before, action, actionId, userBy, "Action");
            }
            var afterAttachments = await _attachmentRepo.GetByEntityAsync("Action", actionId);
            if (beforeAttachments != null && afterAttachments != null)
            {
                await _logService.LogUpdateAsync(beforeAttachments, afterAttachments, actionId, userBy, "Attachment");
            }
            var afterFinding = await _findingRepo.GetFindingByIdAsync(findingId.Value);
            if (beforeFinding != null && afterFinding != null)
            {
                await _logService.LogUpdateAsync(beforeFinding, afterFinding, findingId.Value, userBy, "Finding");
            }

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

        public async Task<AvailableCAPAOwnerResponse> GetAvailableCAPAOwnersAsync(DateTime date, int? deptId = null)
        {
            return await _repo.GetAvailableCAPAOwnersAsync(date, deptId);
        }

    }

}
                                                                                                                                                                                                                            