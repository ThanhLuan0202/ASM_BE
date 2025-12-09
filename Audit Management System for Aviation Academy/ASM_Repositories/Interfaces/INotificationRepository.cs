using ASM_Repositories.Entities;
using ASM_Repositories.Models.NotificationDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<ViewNotification>> GetAllAsync();
        Task<ViewNotification?> GetByIdAsync(Guid notificationId);
        Task<ViewNotification> CreateAsync(CreateNotification dto);
        Task<ViewNotification?> UpdateAsync(Guid notificationId, UpdateNotification dto);
        Task<bool> DeleteAsync(Guid notificationId);
        Task<Notification> CreateNotificationAsync(Notification create);
        Task MarkAsReadAsync(Guid notificationId);
        Task<bool> NotificationExistsAsync(string title, Guid userId, Guid? entityId, string entityType, DateTime? fromDate = null);
    }
}

