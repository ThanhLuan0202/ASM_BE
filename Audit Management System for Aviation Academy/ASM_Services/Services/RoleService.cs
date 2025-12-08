using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.RoleDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repo;
        private readonly IAuditLogService _logService;
        
        public RoleService(IRoleRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewRole>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewRole?> GetByIdAsync(string roleName) => _repo.GetByIdAsync(roleName);
        
        public async Task<ViewRole> CreateAsync(CreateRole dto, Guid userId)
        {
            var created = await _repo.AddAsync(dto);
            var entityId = Guid.TryParse(dto.RoleName, out Guid parsedId) ? parsedId : Guid.NewGuid();
            await _logService.LogCreateAsync(created, entityId, userId, "Role");
            return created;
        }
        
        public async Task<ViewRole?> UpdateAsync(string roleName, UpdateRole dto, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(roleName);
            var updated = await _repo.UpdateAsync(roleName, dto);
            
            if (updated != null && existing != null)
            {
                var entityId = Guid.TryParse(roleName, out Guid parsedId) ? parsedId : Guid.NewGuid();
                await _logService.LogUpdateAsync(existing, updated, entityId, userId, "Role");
            }
            
            return updated;
        }
        
        public async Task<bool> DeleteAsync(string roleName, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(roleName);
            var deleted = await _repo.DeleteAsync(roleName);
            
            if (deleted && existing != null)
            {
                var entityId = Guid.TryParse(roleName, out Guid parsedId) ? parsedId : Guid.NewGuid();
                await _logService.LogDeleteAsync(existing, entityId, userId, "Role");
            }
            
            return deleted;
        }
    }
}
