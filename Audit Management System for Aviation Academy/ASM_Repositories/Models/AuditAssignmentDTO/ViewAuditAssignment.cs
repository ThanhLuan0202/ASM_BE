using System;

namespace ASM_Repositories.Models.AuditAssignmentDTO
{
    public class ViewAuditAssignment
    {
        public Guid AssignmentId { get; set; }

        public Guid AuditId { get; set; }

        public int DeptId { get; set; }

        public Guid AuditorId { get; set; }

        public string Notes { get; set; }

        public DateTime AssignedAt { get; set; }

        public string Status { get; set; }

        // Navigation properties for display
        public string AuditTitle { get; set; }
        public string DepartmentName { get; set; }
        public string AuditorName { get; set; }
    }
}

