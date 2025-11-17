using ASM_Repositories.Models.ActionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IActionRepository
    {
        Task<IEnumerable<ViewAction>> GetAllAsync();
        Task<ViewAction?> GetByIdAsync(Guid id);
        Task<ViewAction> CreateAsync(CreateAction dto);
        Task<ViewAction> UpdateAsync(Guid id, UpdateAction dto);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<bool> UpdateStatusToInProgressAsync(Guid id);
        Task<bool> UpdateStatusToReviewedAsync(Guid id);
        Task<bool> UpdateStatusToApprovedAsync(Guid id);
        Task<bool> UpdateStatusToRejectedAsync(Guid id);
        Task<bool> UpdateStatusToClosedAsync(Guid id);
        Task UpdateActionStatusAsync(Guid actionId, string status);
        Task<Guid?> GetFindingIdByActionIdAsync(Guid actionId);
        Task<IEnumerable<ViewAction>> GetByAssignedToAsync(Guid userId);
    }
}
