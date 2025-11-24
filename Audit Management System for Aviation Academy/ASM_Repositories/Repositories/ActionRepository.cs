using ASM_Repositories.DBContext;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ActionDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class ActionRepository : IActionRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public ActionRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAction>> GetAllAsync()
        {
            var list = await _context.Actions
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAction>>(list);
        }

        public async Task<ViewAction?> GetByIdAsync(Guid id)
        {
            var entity = await _context.Actions
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ActionId == id);

            return _mapper.Map<ViewAction?>(entity);
        }

        public async Task<ViewAction> CreateAsync(CreateAction dto)
        {
            try
            {
                if (!await _context.Findings.AnyAsync(f => f.FindingId == dto.FindingId))
                    throw new ArgumentException($"FindingId '{dto.FindingId}' does not exist.");

                if (dto.AssignedBy.HasValue &&
                    !await _context.UserAccounts.AnyAsync(u => u.UserId == dto.AssignedBy))
                    throw new ArgumentException($"AssignedBy '{dto.AssignedBy}' does not exist.");

                if (dto.AssignedTo.HasValue &&
                    !await _context.UserAccounts.AnyAsync(u => u.UserId == dto.AssignedTo))
                    throw new ArgumentException($"AssignedTo '{dto.AssignedTo}' does not exist.");

                if (dto.AssignedDeptId.HasValue &&
                    !await _context.Departments.AnyAsync(d => d.DeptId == dto.AssignedDeptId))
                    throw new ArgumentException($"AssignedDeptId '{dto.AssignedDeptId}' does not exist.");

                var entity = _mapper.Map<Entities.Action>(dto);
                entity.ActionId = Guid.NewGuid();
                entity.CreatedAt = DateTime.UtcNow;
                entity.Status = "Active";

                _context.Actions.Add(entity);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewAction>(entity);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ActionRepository.AddAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while creating the action.", ex);
            }
        }


        public async Task<ViewAction> UpdateAsync(Guid id, UpdateAction dto)
        {
            try
            {
                var existing = await _context.Actions.FirstOrDefaultAsync(a => a.ActionId == id);
                if (existing == null)
                    throw new ArgumentException($"Action with ID '{id}' does not exist.");

                if (dto.FindingId != Guid.Empty)
                {
                    var exists = await _context.Findings.AnyAsync(f => f.FindingId == dto.FindingId);
                    if (!exists)
                        throw new ArgumentException($"Finding with ID {dto.FindingId} does not exist.");
                }

                if (dto.AssignedTo.HasValue &&
                    !await _context.UserAccounts.AnyAsync(u => u.UserId == dto.AssignedTo))
                    throw new ArgumentException($"AssignedTo '{dto.AssignedTo}' does not exist.");

                if (dto.AssignedDeptId.HasValue &&
                    !await _context.Departments.AnyAsync(d => d.DeptId == dto.AssignedDeptId))
                    throw new ArgumentException($"AssignedDeptId '{dto.AssignedDeptId}' does not exist.");

                _mapper.Map(dto, existing);

                _context.Actions.Update(existing);
                await _context.SaveChangesAsync();

                return _mapper.Map<ViewAction>(existing);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ActionRepository.UpdateAsync] Error: " + ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                throw new Exception("An unexpected error occurred while updating the action.", ex);
            }
        }


        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.Actions.FindAsync(id);
            if (entity == null) return false;

            entity.Status = "Inactive";
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToInProgressAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "InProgress");

            if (!statusExists)
                throw new InvalidOperationException("Status 'InProgress' does not exist in ActionStatus.");

            entity.Status = "InProgress";
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToReviewedAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Reviewed");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Reviewed' does not exist in ActionStatus.");

            entity.Status = "Reviewed";
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToApprovedAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Approved");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Approved' does not exist in ActionStatus.");

            entity.Status = "Approved";
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToRejectedAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Rejected");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Rejected' does not exist in ActionStatus.");

            entity.Status = "Rejected";
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToClosedAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Closed");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Closed' does not exist in ActionStatus.");

            entity.Status = "Closed";
            entity.ClosedAt = DateTime.UtcNow;
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToReturnedAsync(Guid id, string reviewFeedback)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Returned");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Returned' does not exist in ActionStatus.");

            entity.Status = "Returned";
            entity.ReviewFeedback = reviewFeedback;
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToApprovedAsync(Guid id, string reviewFeedback)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Approved");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Approved' does not exist in ActionStatus.");

            entity.Status = "Approved";
            entity.ReviewFeedback = reviewFeedback;
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToCompletedAsync(Guid id, string reviewFeedback)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Completed");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Completed' does not exist in ActionStatus.");

            entity.Status = "Completed";
            entity.ReviewFeedback = reviewFeedback;
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToRejectedAsync(Guid id, string reviewFeedback)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Rejected");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Rejected' does not exist in ActionStatus.");

            entity.Status = "Rejected";
            entity.ReviewFeedback = reviewFeedback;
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToVerifiedAsync(Guid id, string reviewFeedback)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Verified");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Verified' does not exist in ActionStatus.");

            entity.Status = "Verified";
            entity.ReviewFeedback = reviewFeedback;
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToDeclinedAsync(Guid id, string reviewFeedback)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "Declined");

            if (!statusExists)
                throw new InvalidOperationException("Status 'Declined' does not exist in ActionStatus.");

            entity.Status = "Declined";
            entity.ReviewFeedback = reviewFeedback;
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task UpdateActionStatusAsync(Guid actionId, string status)
        {
            var entity = await _context.Actions
                .FirstOrDefaultAsync(x => x.ActionId == actionId);

            if (entity == null)
                throw new Exception("Action not found");

            entity.Status = status;
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Guid?> GetFindingIdByActionIdAsync(Guid actionId)
        {
            return await _context.Actions
                .Where(a => a.ActionId == actionId)
                .Select(a => (Guid?)a.FindingId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ViewAction>> GetByAssignedToAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            var list = await _context.Actions
                .AsNoTracking()
                .Where(a => a.AssignedTo == userId && a.Status != "Inactive")
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAction>>(list);
        }

        public async Task<bool> UpdateProgressPercentAsync(Guid id, byte progressPercent)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            if (progressPercent > 100)
                throw new ArgumentException("ProgressPercent cannot exceed 100.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            if (progressPercent < entity.ProgressPercent)
                throw new InvalidOperationException("ProgressPercent cannot be decreased.");

            entity.ProgressPercent = progressPercent;
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusToApprovedAuditorAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ActionId cannot be empty.");

            var entity = await _context.Actions
                .FirstOrDefaultAsync(a => a.ActionId == id && a.Status != "Inactive");

            if (entity == null)
                return false;

            var statusExists = await _context.ActionStatuses
                .AnyAsync(s => s.ActionStatus1 == "ApprovedAuditor");

            if (!statusExists)
                throw new InvalidOperationException("Status 'ApprovedAuditor' does not exist in ActionStatus.");

            entity.Status = "ApprovedAuditor";
            _context.Actions.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ViewAction>> GetByFindingIdAsync(Guid findingId)
        {
            if (findingId == Guid.Empty)
                throw new ArgumentException("FindingId cannot be empty.");

            var list = await _context.Actions
                .AsNoTracking()
                .Where(a => a.FindingId == findingId && a.Status != "Inactive")
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ViewAction>>(list);
        }
    }
}
