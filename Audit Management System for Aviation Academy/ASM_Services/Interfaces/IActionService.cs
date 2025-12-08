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
        Task<ViewAction> CreateAsync(CreateAction dto);
        Task<ViewAction> UpdateAsync(Guid id, UpdateAction dto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateStatusToInProgressAsync(Guid id);
        Task<bool> UpdateStatusToReviewedAsync(Guid id);
        Task<bool> UpdateStatusToApprovedAsync(Guid id);
        Task<bool> UpdateStatusToRejectedAsync(Guid id);
        Task<bool> UpdateStatusToRejectedAsync(Guid id, string reviewFeedback);
        Task<bool> UpdateStatusToClosedAsync(Guid id);
        Task<IEnumerable<ViewAction>> GetByAssignedToAsync(Guid userId);
        Task<bool> UpdateProgressPercentAsync(Guid id, byte progressPercent);
        Task<List<Notification>> ActionVerifiedAsync(Guid actionId, Guid userBy, string reviewFeedback);
        Task<Notification> ActionDeclinedAsync(Guid actionId, Guid userBy, string reviewFeedback);
        Task<List<Notification>> ActionApprovedAsync(Guid actionId, Guid userBy, string reviewFeedback);
        Task<Notification> ActionRejectedAsync(Guid actionId, Guid userBy, string reviewFeedback);
        Task<List<Notification>> ApproveByHigherLevel(Guid actionId, Guid userBy, string reviewFeedback);
        Task<List<Notification>> RejectByHigherLevel(Guid actionId, Guid userBy, string reviewFeedback);
        Task<bool> UpdateStatusToApprovedAuditorAsync(Guid id);
        Task<IEnumerable<ViewAction>> GetByFindingIdAsync(Guid findingId);
        Task<IEnumerable<ViewAction>> GetByAssignedDeptIdAsync(int assignedDeptId);
        Task<AvailableCAPAOwnerResponse> GetAvailableCAPAOwnersAsync(DateTime date, int? deptId = null);
    }
}
