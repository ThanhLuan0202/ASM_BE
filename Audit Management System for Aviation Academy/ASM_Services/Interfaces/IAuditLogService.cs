using ASM_Repositories.Models.AuditLogDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.AdminInterfaces
{
    public interface IAuditLogService
    {
        Task<IEnumerable<ViewAuditLog>> GetAllAsync();
        Task<ViewAuditLog?> GetByIdAsync(Guid logId);
        Task<ViewAuditLog> CreateAsync(CreateAuditLog dto);
        Task<ViewAuditLog?> UpdateAsync(Guid logId, UpdateAuditLog dto);
        Task<bool> DeleteAsync(Guid logId);
        Task LogCreateAsync<T>(T newData, Guid entityId, Guid userId, string entityType);
        Task LogUpdateAsync<T>(T oldData, T newData, Guid entityId, Guid userId, string entityType);
        Task LogSoftDeleteAsync<T>(T oldData, T newData, Guid entityId, Guid userId, string entityType);
        Task LogDeleteAsync<T>(T oldData, Guid entityId, Guid userId, string entityType);

    }
}

