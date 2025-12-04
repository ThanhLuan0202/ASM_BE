using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.ChecklistTemplateDTO
{
    public class UpdateChecklistTemplate
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; }

        [MaxLength(50, ErrorMessage = "Version cannot exceed 50 characters")]
        public string Version { get; set; }

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public int? DeptId { get; set; }
        public string Status { get; set; }

        public bool IsActive { get; set; }
    }
}
