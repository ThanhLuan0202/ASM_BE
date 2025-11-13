using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ActionStatusDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class ActionStatusService : IActionStatusService
    {
        private readonly IActionStatusRepository _repo;

        public ActionStatusService(IActionStatusRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewActionStatus>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewActionStatus?> GetByIdAsync(string actionStatus) => _repo.GetByIdAsync(actionStatus);
        public Task<ViewActionStatus> CreateAsync(CreateActionStatus dto) => _repo.CreateAsync(dto);
        public Task<ViewActionStatus?> UpdateAsync(string actionStatus, UpdateActionStatus dto) => _repo.UpdateAsync(actionStatus, dto);
        public Task<bool> DeleteAsync(string actionStatus) => _repo.DeleteAsync(actionStatus);
    }
}
