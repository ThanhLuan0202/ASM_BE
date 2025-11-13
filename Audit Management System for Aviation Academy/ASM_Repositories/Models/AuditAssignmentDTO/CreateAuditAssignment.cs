using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AuditAssignmentDTO
{
    public class CreateAuditAssignment
    {
        [Required(ErrorMessage = "AuditId is required")]
        public Guid AuditId { get; set; }

        [Required(ErrorMessage = "DeptId is required")]
        public int DeptId { get; set; }

        [Required(ErrorMessage = "AuditorId is required")]
        public Guid AuditorId { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string Notes { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }
    }
}

