using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.UsersDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repo;
        private readonly IAuditLogService _logService;

        public UsersService(IUsersRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }
        public Task<IEnumerable<ViewUser>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewUser> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        
        public async Task<ViewUser> CreateAsync(CreateUser dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            await _logService.LogCreateAsync(created, created.UserId, userId, "User");
            return created;
        }
        
        public async Task<ViewUser> UpdateAsync(Guid id, UpdateUser dto, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateAsync(id, dto);
            
            if (updated != null && existing != null)
            {
                await _logService.LogUpdateAsync(existing, updated, id, userId, "User");
            }
            
            return updated;
        }
        
        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var existing = await _repo.GetByIdAsync(id);
            var deleted = await _repo.DeleteAsync(id);
            
            if (deleted && existing != null)
            {
                await _logService.LogDeleteAsync(existing, id, userId, "User");
            }
            
            return deleted;
        }
        
        public Task<IEnumerable<ViewUser>> GetByDeptIdAsync(int deptId) => _repo.GetByDeptIdAsync(deptId);

        public Task<Guid?> GetLeadAuditorIdAsync() => _repo.GetLeadAuditorIdAsync();
    }
}