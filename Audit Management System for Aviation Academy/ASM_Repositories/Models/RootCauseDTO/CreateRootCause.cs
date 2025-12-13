using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.RootCauseDTO
{
    public class CreateRootCause
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Active";

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        public int? DeptId { get; set; }

        public Guid? FindingId { get; set; }
    }
}
