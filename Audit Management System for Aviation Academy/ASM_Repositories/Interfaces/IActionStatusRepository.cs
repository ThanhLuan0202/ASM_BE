using ASM_Repositories.Models.ActionStatusDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IActionStatusRepository
    {
        Task<IEnumerable<ViewActionStatus>> GetAllAsync();
        Task<ViewActionStatus?> GetByIdAsync(string actionStatus);
        Task<ViewActionStatus> CreateAsync(CreateActionStatus dto);
        Task<ViewActionStatus?> UpdateAsync(string actionStatus, UpdateActionStatus dto);
        Task<bool> DeleteAsync(string actionStatus);
    }
}
