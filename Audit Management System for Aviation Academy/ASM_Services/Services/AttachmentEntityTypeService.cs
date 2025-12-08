using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AttachmentEntityTypeDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AttachmentEntityTypeService : IAttachmentEntityTypeService
    {
        private readonly IAttachmentEntityTypeRepository _repo;
        private readonly IAuditLogService _logService;

        public AttachmentEntityTypeService(IAttachmentEntityTypeRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewAttachmentEntityType>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAttachmentEntityType?> GetByIdAsync(string entityType) => _repo.GetByIdAsync(entityType);
        public async Task<ViewAttachmentEntityType> CreateAsync(CreateAttachmentEntityType dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            await _logService.LogCreateAsync(created, Guid.Empty, userId, "AttachmentEntityType");
            return created;
        }
        public async Task<ViewAttachmentEntityType?> UpdateAsync(string entityType, UpdateAttachmentEntityType dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(entityType);
            var updated = await _repo.UpdateAsync(entityType, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, Guid.Empty, userId, "AttachmentEntityType");
            }
            return updated;
        }
        public async Task<bool> DeleteAsync(string entityType, Guid userId)
        {
            var before = await _repo.GetByIdAsync(entityType);
            var success = await _repo.DeleteAsync(entityType);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, Guid.Empty, userId, "AttachmentEntityType");
            }
            return success;
        }
    }
}

