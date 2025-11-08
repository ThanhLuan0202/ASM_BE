using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.NotificationDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services.AdminServices
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
    }
}

