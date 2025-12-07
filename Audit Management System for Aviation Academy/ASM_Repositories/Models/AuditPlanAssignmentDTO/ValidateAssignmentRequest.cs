using System;

namespace ASM_Repositories.Models.AuditPlanAssignmentDTO
{
    public class ValidateAssignmentRequest
    {
        public Guid AuditorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}

