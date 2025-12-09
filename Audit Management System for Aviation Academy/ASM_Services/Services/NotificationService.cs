using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.NotificationDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;

        public NotificationService(INotificationRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewNotification>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewNotification?> GetByIdAsync(Guid notificationId) => _repo.GetByIdAsync(notificationId);
        public Task<ViewNotification> CreateAsync(CreateNotification dto) => _repo.CreateAsync(dto);
        public Task<ViewNotification?> UpdateAsync(Guid notificationId, UpdateNotification dto) => _repo.UpdateAsync(notificationId, dto);
        public Task<bool> DeleteAsync(Guid notificationId) => _repo.DeleteAsync(notificationId);
        public async Task MarkAsReadAsync(Guid notificationId) => await _repo.MarkAsReadAsync(notificationId);
        public Task<Notification> CreateNotificationAsync(Notification create) => _repo.CreateNotificationAsync(create);
        public Task<bool> NotificationExistsAsync(string title, Guid userId, Guid? entityId, string entityType, DateTime? fromDate = null) 
            => _repo.NotificationExistsAsync(title, userId, entityId, entityType, fromDate);
    }
}

