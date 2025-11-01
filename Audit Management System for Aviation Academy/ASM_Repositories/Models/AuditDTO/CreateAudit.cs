using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AuditDTO
{
    public class CreateAudit
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(300, ErrorMessage = "Title cannot exceed 300 characters")]
        public string Title { get; set; }

        [MaxLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
        public string Type { get; set; }

        [MaxLength(500, ErrorMessage = "Scope cannot exceed 500 characters")]
        public string Scope { get; set; }

        public Guid? TemplateId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        public Guid? CreatedBy { get; set; }

        public bool IsPublished { get; set; }

        [MaxLength(1000, ErrorMessage = "Objective cannot exceed 1000 characters")]
        public string Objective { get; set; }
    }
}
