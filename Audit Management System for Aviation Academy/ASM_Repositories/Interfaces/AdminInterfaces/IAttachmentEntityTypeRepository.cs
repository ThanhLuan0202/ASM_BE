using ASM_Repositories.Models.AttachmentEntityTypeDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.AdminInterfaces
{
    public interface IAttachmentEntityTypeRepository
    {
        Task<IEnumerable<ViewAttachmentEntityType>> GetAllAsync();
        Task<ViewAttachmentEntityType?> GetByIdAsync(string entityType);
        Task<ViewAttachmentEntityType> CreateAsync(CreateAttachmentEntityType dto);
        Task<ViewAttachmentEntityType?> UpdateAsync(string entityType, UpdateAttachmentEntityType dto);
        Task<bool> DeleteAsync(string entityType);
    }
}

