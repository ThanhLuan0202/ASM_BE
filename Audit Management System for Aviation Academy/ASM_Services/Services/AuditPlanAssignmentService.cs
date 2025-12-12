using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditPlanAssignmentService : IAuditPlanAssignmentService
    {
        private readonly IAuditPlanAssignmentRepository _repo;
        private readonly IUsersRepository _userRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly IAuditLogService _logService;
        private readonly IFirebaseUploadService _firebaseUploadService;

        public AuditPlanAssignmentService(IAuditPlanAssignmentRepository repo, IUsersRepository usersRepo, INotificationRepository notificationRepo, IAuditLogService logService, IFirebaseUploadService firebaseUploadService)
        {
            _repo = repo;
            _notificationRepo = notificationRepo;
            _userRepo = usersRepo;
            _logService = logService;
            _firebaseUploadService = firebaseUploadService;
        }

        public Task<IEnumerable<ViewAuditPlanAssignment>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditPlanAssignment?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public async Task<(ViewAuditPlanAssignment Assignment, Notification? Notification)> CreateWithNotificationAsync(CreateAuditPlanAssignment dto, Guid userId, List<IFormFile> files)
        {
            // Upload files if provided
            string filePathsJson = null;
            if (files != null && files.Any())
            {
                var filePaths = new List<string>();
                
                // Upload files lên Firebase
                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var blobPath = await _firebaseUploadService.UploadFileAsync(file, "AuditPlanAssignments");
                        filePaths.Add(blobPath);
                    }
                }
                
                // Serialize file paths thành JSON
                if (filePaths.Any())
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        WriteIndented = false
                    };
                    filePathsJson = JsonSerializer.Serialize(filePaths, jsonOptions);
                }
            }

            var assignment = await _repo.CreateAsync(dto, filePathsJson);
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

            await _logService.LogCreateAsync(assignment, assignment.AssignmentId, userId, "AuditPlanAssignment");

            return (assignment, notification);
        }

        public async Task<ViewAuditPlanAssignment?> UpdateAsync(Guid id, UpdateAuditPlanAssignment dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateAsync(id, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, id, userId, "AuditPlanAssignment");
            }
            return updated;
        }
        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var success = await _repo.DeleteAsync(id);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, id, userId, "AuditPlanAssignment");
            }
            return success;
        }
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
