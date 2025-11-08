using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AttachmentDTO
{
    public class UpdateAttachment
    {
        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        public DateOnly? RetentionUntil { get; set; }

        public bool IsArchived { get; set; }
    }
}

