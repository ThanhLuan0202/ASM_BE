using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AuditAssignmentDTO
{
    public class UpdateAuditAssignment
    {
        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }
    }
}

