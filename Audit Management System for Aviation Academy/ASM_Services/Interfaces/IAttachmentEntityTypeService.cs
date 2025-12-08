using ASM_Repositories.Models.AttachmentEntityTypeDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.AdminInterfaces
{
    public interface IAttachmentEntityTypeService
    {
        Task<IEnumerable<ViewAttachmentEntityType>> GetAllAsync();
        Task<ViewAttachmentEntityType?> GetByIdAsync(string entityType);
        Task<ViewAttachmentEntityType> CreateAsync(CreateAttachmentEntityType dto, Guid userId);
        Task<ViewAttachmentEntityType?> UpdateAsync(string entityType, UpdateAttachmentEntityType dto, Guid userId);
        Task<bool> DeleteAsync(string entityType, Guid userId);
    }
}

