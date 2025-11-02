using ASM_Repositories.Models.ActionDTO;
using ASM_Services.Interfaces.DepartmentHeadInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services.DepartmentHeadServices
{
    public class ActionService : IActionService
    {
        private readonly IActionRepository _repo;

        public ActionService(IActionRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAction>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAction?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewAction> CreateAsync(CreateAction dto) => _repo.CreateAsync(dto);
        public Task<ViewAction> UpdateAsync(Guid id, UpdateAction dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> DeleteAsync(Guid id) => _repo.SoftDeleteAsync(id);
    }

}
                                                                                                                                                                                                                            