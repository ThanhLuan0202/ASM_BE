using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.NotificationDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public NotificationRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewNotification>> GetAllAsync()
        {
            var data = await _context.Notifications
                .Include(x => x.User)
                .Include(x => x.EntityTypeNavigation)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ViewNotification>>(data);
        }

        public async Task<ViewNotification?> GetByIdAsync(Guid notificationId)
        {
            var entity = await _context.Notifications
                .Include(x => x.User)
                .Include(x => x.EntityTypeNavigation)
                .FirstOrDefaultAsync(x => x.NotificationId == notificationId);
            return entity == null ? null : _mapper.Map<ViewNotification>(entity);
        }

        public async Task<ViewNotification> CreateAsync(CreateNotification dto)
        {
            var userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

            var entityTypeExists = await _context.AttachmentEntityTypes.AnyAsync(e => e.EntityType == dto.EntityType);
            if (!entityTypeExists)
                throw new InvalidOperationException($"EntityType '{dto.EntityType}' does not exist.");

            var entity = _mapper.Map<Notification>(dto);
            entity.NotificationId = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            
            if (string.IsNullOrWhiteSpace(entity.Status))
                entity.Status = "Active";

            if (entity.IsRead && entity.ReadAt == null)
                entity.ReadAt = DateTime.UtcNow;

            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.Notifications
                .Include(x => x.User)
                .Include(x => x.EntityTypeNavigation)
                .FirstOrDefaultAsync(x => x.NotificationId == entity.NotificationId);

            return _mapper.Map<ViewNotification>(created);
        }

        public async Task<ViewNotification?> UpdateAsync(Guid notificationId, UpdateNotification dto)
        {
            var entity = await _context.Notifications
                .FirstOrDefaultAsync(x => x.NotificationId == notificationId);

            if (entity == null) return null;

            var userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == dto.UserId);
            if (!userExists)
                throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

            var entityTypeExists = await _context.AttachmentEntityTypes.AnyAsync(e => e.EntityType == dto.EntityType);
            if (!entityTypeExists)
                throw new InvalidOperationException($"EntityType '{dto.EntityType}' does not exist.");

            if (dto.IsRead && !entity.IsRead)
            {
                entity.ReadAt = DateTime.UtcNow;
            }
            else if (!dto.IsRead && entity.IsRead)
            {
                entity.ReadAt = null;
            }

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            var updated = await _context.Notifications
                .Include(x => x.User)
                .Include(x => x.EntityTypeNavigation)
                .FirstOrDefaultAsync(x => x.NotificationId == notificationId);

            return _mapper.Map<ViewNotification>(updated);
        }

        public async Task<bool> DeleteAsync(Guid notificationId)
        {
            var entity = await _context.Notifications.FindAsync(notificationId);
            if (entity == null) return false;

            entity.Status = "Inactive";
            _context.Entry(entity).Property(p => p.Status).IsModified = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Notification> CreateNotificationAsync(Notification create)
        {
            if (create == null)
                throw new ArgumentNullException(nameof(create));

            create.NotificationId = Guid.NewGuid();            
            create.CreatedAt = DateTime.UtcNow;    
            create.IsRead = false;                 

            await _context.Notifications.AddAsync(create);
            await _context.SaveChangesAsync();

            return create; 
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
                throw new ArgumentException("NotificationId cannot be empty.");

            var notif = await _context.Notifications.FirstOrDefaultAsync(a => a.NotificationId == notificationId);
            if (notif == null)
                throw new InvalidOperationException($"No Notification found for NotificationId '{notificationId}'.");

            notif.IsRead = true;
            notif.ReadAt = DateTime.UtcNow;
            _context.Entry(notif).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> NotificationExistsAsync(string title, Guid userId, Guid? entityId, string entityType, DateTime? fromDate = null)
        {
            var query = _context.Notifications
                .Where(n => n.Title == title
                    && n.UserId == userId
                    && n.EntityType == entityType
                    && n.Status == "Active");

            if (entityId.HasValue)
            {
                query = query.Where(n => n.EntityId == entityId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt >= fromDate.Value);
            }

            return await query.AnyAsync();
        }

    }
}

