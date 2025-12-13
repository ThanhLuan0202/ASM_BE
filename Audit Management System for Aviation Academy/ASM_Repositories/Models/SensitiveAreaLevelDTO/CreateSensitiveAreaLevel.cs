using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.SensitiveAreaLevelDTO
{
    public class CreateSensitiveAreaLevel
    {
        [Required(ErrorMessage = "Level is required")]
        [MaxLength(50, ErrorMessage = "Level cannot exceed 50 characters")]
        public string Level { get; set; }
    }
}

