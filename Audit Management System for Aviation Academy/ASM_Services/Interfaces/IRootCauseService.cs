using ASM_Repositories.Models.RootCauseDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.SQAStaffInterfaces
{
    public interface IRootCauseService
    {
        Task<IEnumerable<ViewRootCause>> GetAllAsync();
        Task<IEnumerable<ViewRootCause>> GetByStatusAsync(string status);
        Task<IEnumerable<ViewRootCause>> GetByCategoryAsync(string category);
        Task<IEnumerable<ViewRootCause>> GetByDeptIdAsync(int deptId);
        Task<IEnumerable<ViewRootCause>> GetByFindingIdAsync(Guid findingId);
        Task<ViewRootCause?> GetByIdAsync(int id);
        Task<ViewRootCause> CreateAsync(CreateRootCause dto);
        Task<ViewRootCause?> UpdateAsync(int id, UpdateRootCause dto);
        Task<bool> DeleteAsync(int id);
    }
}
