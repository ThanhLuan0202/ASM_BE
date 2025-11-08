using System;

namespace ASM_Repositories.Models.AuditLogDTO
{
    public class ViewAuditLog
    {
        public Guid LogId { get; set; }
        public string EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string Action { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Role { get; set; }
        public Guid? PerformedBy { get; set; }
        public DateTime PerformedAt { get; set; }
    }
}

