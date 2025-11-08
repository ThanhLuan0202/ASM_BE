using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AttachmentDTO
{
    public class CreateAttachment
    {
        [Required(ErrorMessage = "EntityType is required")]
        [MaxLength(50, ErrorMessage = "EntityType cannot exceed 50 characters")]
        public string EntityType { get; set; }

        [Required(ErrorMessage = "EntityId is required")]
        public Guid EntityId { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        public DateOnly? RetentionUntil { get; set; }

        public bool IsArchived { get; set; }
    }
}

