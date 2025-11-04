using ASM_Repositories.Interfaces.SQAStaffInterfaces;
using ASM_Repositories.Models.RootCauseDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services.SQAStaffServices
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
