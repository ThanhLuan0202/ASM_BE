using ASM_Repositories.Entities;
using ASM_Repositories.Models.ActionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IActionService
    {
        Task<IEnumerable<ViewAction>> GetAllAsync();
        Task<ViewAction?> GetByIdAsync(Guid id);
        Task<ViewAction> CreateAsync(CreateAction dto, Guid userId);
        Task<ViewAction> UpdateAsync(Guid id, UpdateAction dto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
        Task<bool> UpdateStatusToInProgressAsync(Guid id, Guid userId);
        Task<bool> UpdateStatusToReviewedAsync(Guid id, Guid userId);
        Task<bool> UpdateStatusToApprovedAsync(Guid id, Guid userId);
        Task<bool> UpdateStatusToRejectedAsync(Guid id, Guid userId);
        Task<bool> UpdateStatusToRejectedAsync(Guid id, string reviewFeedback, Guid userId);
        Task<bool> UpdateStatusToClosedAsync(Guid id, Guid userId);
        Task<IEnumerable<ViewAction>> GetByAssignedToAsync(Guid userId);
        Task<bool> UpdateProgressPercentAsync(Guid id, byte progressPercent, Guid userId);
        Task<List<Notification>> ActionVerifiedAsync(Guid actionId, Guid userBy, string reviewFeedback);
        Task<Notification> ActionDeclinedAsync(Guid actionId, Guid userBy, string reviewFeedback);
        Task<List<Notification>> ActionApprovedAsync(Guid actionId, Guid userBy, string reviewFeedback);
        Task<Notification> ActionRejectedAsync(Guid actionId, Guid userBy, string reviewFeedback);
        Task<List<Notification>> ApproveByHigherLevel(Guid actionId, Guid userBy, string reviewFeedback);
        Task<List<Notification>> RejectByHigherLevel(Guid actionId, Guid userBy, string reviewFeedback);
        Task<bool> UpdateStatusToApprovedAuditorAsync(Guid id, Guid userId);
        Task<IEnumerable<ViewAction>> GetByFindingIdAsync(Guid findingId);
        Task<IEnumerable<ViewAction>> GetByAssignedDeptIdAsync(int assignedDeptId);
        Task<AvailableCAPAOwnerResponse> GetAvailableCAPAOwnersAsync(DateTime date, int? deptId = null);
    }
}
