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

        public ActionService(IActionRepository repo)
        {
            _repo = repo;
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
    }

}
                                                                                                                                                                                                                            