using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.DepartmentSensitiveAreaDTO
{
    public class UpdateDepartmentSensitiveArea
    {
        public string SensitiveArea { get; set; }

        [MaxLength(50, ErrorMessage = "Level cannot exceed 50 characters")]
        public string Level { get; set; }

        [MaxLength(500, ErrorMessage = "DefaultNotes cannot exceed 500 characters")]
        public string DefaultNotes { get; set; }
    }
}

