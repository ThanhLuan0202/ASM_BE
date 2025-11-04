using ASM_Repositories.Models.RootCauseDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces.SQAStaffInterfaces
{
    public interface IRootCauseRepository
    {
        Task<IEnumerable<ViewRootCause>> GetAllAsync();
        Task<ViewRootCause?> GetByIdAsync(int id);
        Task<ViewRootCause> CreateAsync(CreateRootCause dto);
        Task<ViewRootCause?> UpdateAsync(int id, UpdateRootCause dto);
        Task<bool> DeleteAsync(int id); // soft delete: set Status = "Inactive"
        Task<bool> ExistsAsync(int id);
    }
}
