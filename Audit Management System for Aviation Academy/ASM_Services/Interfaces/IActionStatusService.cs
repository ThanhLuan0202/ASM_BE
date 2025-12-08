using ASM_Repositories.Models.ActionStatusDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.AdminInterfaces
{
    public interface IActionStatusService
    {
        Task<IEnumerable<ViewActionStatus>> GetAllAsync();
        Task<ViewActionStatus?> GetByIdAsync(string actionStatus);
        Task<ViewActionStatus> CreateAsync(CreateActionStatus dto, Guid userId);
        Task<ViewActionStatus?> UpdateAsync(string actionStatus, UpdateActionStatus dto, Guid userId);
        Task<bool> DeleteAsync(string actionStatus, Guid userId);
    }
}
