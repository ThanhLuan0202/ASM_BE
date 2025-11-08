using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AttachmentEntityTypeDTO
{
    public class CreateAttachmentEntityType
    {
        [Required(ErrorMessage = "EntityType is required")]
        [MaxLength(50, ErrorMessage = "EntityType cannot exceed 50 characters")]
        public string EntityType { get; set; }
    }
}

