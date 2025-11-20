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
        Task<bool> UpdateStatusToClosedAsync(Guid id);
        Task<IEnumerable<ViewAction>> GetByAssignedToAsync(Guid userId);
        Task<bool> UpdateProgressPercentAsync(Guid id, byte progressPercent);
        Task<Notification> ActionApprovedAsync(Guid actionId, Guid rejectedBy, string reviewFeedback);
        Task<Notification> ActionRejectedAsync(Guid actionId, Guid rejectedBy, string reviewFeedback);
        Task ApproveByHigherLevel(Guid actionId, string reviewFeedback);
        Task RejectByHigherLevel(Guid actionId, string reviewFeedback);
    }
}
