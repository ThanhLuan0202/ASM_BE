using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.DepartmentSensitiveAreaDTO
{
    public class CreateDepartmentSensitiveArea
    {
        [Required(ErrorMessage = "DeptId is required")]
        public int DeptId { get; set; }

        [Required(ErrorMessage = "SensitiveAreas is required")]
        public List<string> SensitiveAreas { get; set; }

        [MaxLength(500, ErrorMessage = "DefaultNotes cannot exceed 500 characters")]
        public string DefaultNotes { get; set; }
    }
}

