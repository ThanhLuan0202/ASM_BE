using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.NotificationDTO
{
    public class UpdateNotification
    {
        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }

        [Required(ErrorMessage = "EntityType is required")]
        [MaxLength(50, ErrorMessage = "EntityType cannot exceed 50 characters")]
        public string EntityType { get; set; }

        public Guid? EntityId { get; set; }

        public bool IsRead { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }
    }
}

