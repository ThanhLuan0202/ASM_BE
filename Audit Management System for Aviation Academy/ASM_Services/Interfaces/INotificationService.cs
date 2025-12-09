using ASM_Repositories.Entities;
using ASM_Repositories.Models.NotificationDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.AdminInterfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<ViewNotification>> GetAllAsync();
        Task<ViewNotification?> GetByIdAsync(Guid notificationId);
        Task<ViewNotification> CreateAsync(CreateNotification dto);
        Task<ViewNotification?> UpdateAsync(Guid notificationId, UpdateNotification dto);
        Task<bool> DeleteAsync(Guid notificationId);
        Task MarkAsReadAsync(Guid notificationId);
        Task<Notification> CreateNotificationAsync(Notification create);
    }
}

