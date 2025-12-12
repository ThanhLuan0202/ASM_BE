using System;
using System.Collections.Generic;

namespace ASM_Repositories.Models.AuditAssignmentDTO
{
    public class BulkCreateAuditAssignmentResponse
    {
        public Guid AssignmentId { get; set; }
        public Guid AuditorId { get; set; }
        public string Status { get; set; }
    }
}

