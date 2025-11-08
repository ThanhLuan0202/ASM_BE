using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.AttachmentEntityTypeDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services.AdminServices
{
    public class AttachmentEntityTypeService : IAttachmentEntityTypeService
    {
        private readonly IAttachmentEntityTypeRepository _repo;

        public AttachmentEntityTypeService(IAttachmentEntityTypeRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAttachmentEntityType>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAttachmentEntityType?> GetByIdAsync(string entityType) => _repo.GetByIdAsync(entityType);
        public Task<ViewAttachmentEntityType> CreateAsync(CreateAttachmentEntityType dto) => _repo.CreateAsync(dto);
        public Task<ViewAttachmentEntityType?> UpdateAsync(string entityType, UpdateAttachmentEntityType dto) => _repo.UpdateAsync(entityType, dto);
        public Task<bool> DeleteAsync(string entityType) => _repo.DeleteAsync(entityType);
    }
}

