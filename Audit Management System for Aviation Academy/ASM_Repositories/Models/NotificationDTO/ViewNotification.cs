using System;

namespace ASM_Repositories.Models.NotificationDTO
{
    public class ViewNotification
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}

