using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AuditAssignmentDTO
{
    public class BulkCreateAuditAssignmentRequest
    {
        [Required(ErrorMessage = "AuditId is required")]
        public Guid AuditId { get; set; }

        [Required(ErrorMessage = "DeptId is required")]
        public int DeptId { get; set; }

        [Required(ErrorMessage = "AuditorIds is required")]
        [MinLength(1, ErrorMessage = "At least one auditor ID is required")]
        public List<Guid> AuditorIds { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }
    }
}

