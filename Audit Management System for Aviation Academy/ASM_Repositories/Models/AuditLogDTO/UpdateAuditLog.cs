using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AuditLogDTO
{
    public class UpdateAuditLog
    {
        [Required(ErrorMessage = "EntityType is required")]
        [MaxLength(50, ErrorMessage = "EntityType cannot exceed 50 characters")]
        public string EntityType { get; set; }

        public Guid? EntityId { get; set; }

        [Required(ErrorMessage = "Action is required")]
        [MaxLength(100, ErrorMessage = "Action cannot exceed 100 characters")]
        public string Action { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        [MaxLength(50, ErrorMessage = "Role cannot exceed 50 characters")]
        public string Role { get; set; }

        public Guid? PerformedBy { get; set; }
    }
}

