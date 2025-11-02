using ASM_Repositories.Models.ActionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces.DepartmentHeadInterfaces
{
    public interface IActionService
    {
        Task<IEnumerable<ViewAction>> GetAllAsync();
        Task<ViewAction?> GetByIdAsync(Guid id);
        Task<ViewAction> CreateAsync(CreateAction dto);
        Task<ViewAction> UpdateAsync(Guid id, UpdateAction dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
