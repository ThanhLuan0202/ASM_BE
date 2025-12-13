using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.RootCauseDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class RootCauseService : IRootCauseService
    {
        private readonly IRootCauseRepository _repo;

        public RootCauseService(IRootCauseRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ViewRootCause>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<IEnumerable<ViewRootCause>> GetByStatusAsync(string status)
        {
            return await _repo.GetByStatusAsync(status);
        }

        public async Task<IEnumerable<ViewRootCause>> GetByCategoryAsync(string category)
        {
            return await _repo.GetByCategoryAsync(category);
        }

        public async Task<IEnumerable<ViewRootCause>> GetByDeptIdAsync(int deptId)
        {
            return await _repo.GetByDeptIdAsync(deptId);
        }

        public async Task<IEnumerable<ViewRootCause>> GetByFindingIdAsync(Guid findingId)
        {
            return await _repo.GetByFindingIdAsync(findingId);
        }

        public async Task<ViewRootCause?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<ViewRootCause> CreateAsync(CreateRootCause dto)
        {
            return await _repo.CreateAsync(dto);
        }

        public async Task<ViewRootCause?> UpdateAsync(int id, UpdateRootCause dto)
        {
            return await _repo.UpdateAsync(id, dto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}
