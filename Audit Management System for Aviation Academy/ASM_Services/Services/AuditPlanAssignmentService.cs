using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidateAssignmentRequest = ASM_Repositories.Models.AuditPlanAssignmentDTO.ValidateAssignmentRequest;
using ValidateAssignmentResponse = ASM_Repositories.Models.AuditPlanAssignmentDTO.ValidateAssignmentResponse;

namespace ASM_Services.Services
{
    public class AuditPlanAssignmentService : IAuditPlanAssignmentService
    {
        private readonly IAuditPlanAssignmentRepository _repo;

        public AuditPlanAssignmentService(IAuditPlanAssignmentRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditPlanAssignment>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditPlanAssignment?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewAuditPlanAssignment> CreateAsync(CreateAuditPlanAssignment dto) => _repo.CreateAsync(dto);
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
