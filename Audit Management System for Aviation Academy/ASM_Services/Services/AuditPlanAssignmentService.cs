using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditPlanAssignmentService : IAuditPlanAssignmentService
    {
        private readonly IAuditPlanAssignmentRepository _repo;
        private readonly IUsersRepository _userRepo;
        private readonly INotificationRepository _notificationRepo;

        public AuditPlanAssignmentService(IAuditPlanAssignmentRepository repo, IUsersRepository usersRepo, INotificationRepository notificationRepo)
        {
            _repo = repo;
            _notificationRepo = notificationRepo;
            _userRepo = usersRepo;
        }

        public Task<IEnumerable<ViewAuditPlanAssignment>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditPlanAssignment?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public async Task<Notification?> CreateWithNotificationAsync(CreateAuditPlanAssignment dto)
        {
            var assignment = await _repo.CreateAsync(dto);
            if (assignment == null)
                throw new Exception("Failed to create audit plan assignment.");

            var assignByUser = await _userRepo.GetUserShortInfoAsync(dto.AssignBy);
            if (assignByUser == null)
                throw new Exception($"AssignBy user not found: {dto.AssignBy}");

            var auditorUser = await _userRepo.GetUserShortInfoAsync(dto.AuditorId);
            if (auditorUser == null)
                throw new Exception($"Auditor user not found: {dto.AuditorId}");

            var message =$"You have been assigned a task to create an audit plan by {assignByUser.FullName} ({assignByUser.RoleName}).\n" +
                        $"Assigned Date: {dto.AssignedDate:yyyy-MM-dd}\n" +
                        (!string.IsNullOrWhiteSpace(dto.Remarks) ? $"Remarks: {dto.Remarks}" : "");

            var notification = await _notificationRepo.CreateNotificationAsync(new Notification
            {
                UserId = auditorUser.UserId,
                Title = "Audit Plan Assignment",
                Message = message,
                EntityType = "AuditPlanAssignment",
                EntityId = assignment.AssignmentId,
                IsRead = false,
                Status = "Active"
            });

            return notification;
        }

        public Task<ViewAuditPlanAssignment?> UpdateAsync(Guid id, UpdateAuditPlanAssignment dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> DeleteAsync(Guid id) => _repo.DeleteAsync(id);
        public Task<IEnumerable<ViewAuditPlanAssignment>> GetAssignmentsByPeriodAsync(DateTime startDate, DateTime endDate) => _repo.GetAssignmentsByPeriodAsync(startDate, endDate);

        public async Task<ValidateAssignmentResponse> ValidateAssignmentAsync(ValidateAssignmentRequest request)
        {
            var now = DateTime.UtcNow;
            var isPeriodExpired = request.EndDate < now;
            
            // Đếm số assignments trong thời kỳ (số auditors đã được assign và đã tạo audits)
            var currentCount = await _repo.GetAssignmentCountByPeriodAsync(request.StartDate, request.EndDate);
            const int maxAllowed = 5;
            
            var canCreate = !isPeriodExpired && currentCount < maxAllowed;
            var reason = string.Empty;

            if (isPeriodExpired)
            {
                reason = "Period has expired. Cannot create new assignments.";
            }
            else if (currentCount >= maxAllowed)
            {
                reason = $"Maximum {maxAllowed} audits allowed in this period. Current count: {currentCount}.";
            }
            else
            {
                reason = "Assignment can be created.";
            }

            return new ValidateAssignmentResponse
            {
                CanCreate = canCreate,
                Reason = reason,
                CurrentCount = currentCount,
                MaxAllowed = maxAllowed,
                IsPeriodExpired = isPeriodExpired
            };
        }
    }
}
