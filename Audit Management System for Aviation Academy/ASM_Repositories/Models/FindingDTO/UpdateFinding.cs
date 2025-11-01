using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.FindingDTO
{
    public class UpdateFinding
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(300, ErrorMessage = "Title cannot exceed 300 characters")]
        public string Title { get; set; }

        public string Description { get; set; }

        [MaxLength(20, ErrorMessage = "Severity cannot exceed 20 characters")]
        public string Severity { get; set; }

        public int? RootCauseId { get; set; }

        public int? DeptId { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        public DateTime? Deadline { get; set; }

        public Guid? ReviewerId { get; set; }

        [MaxLength(50, ErrorMessage = "Source cannot exceed 50 characters")]
        public string Source { get; set; }

        [MaxLength(200, ErrorMessage = "ExternalAuditorName cannot exceed 200 characters")]
        public string ExternalAuditorName { get; set; }

        public Guid? AuditItemId { get; set; }
    }
}
