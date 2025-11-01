using ASM_Repositories.Interfaces.SQAStaffInterfaces;
using ASM_Repositories.Models.FindingDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_Services.Services.SQAStaffServices
{
    public class FindingService : IFindingService
    {
        private readonly IFindingRepository _repo;
        
        public FindingService(IFindingRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<ViewFinding>> GetAllFindingAsync()
        {
            return await _repo.GetAllFindingAsync();
        }

        public async Task<ViewFinding?> GetFindingByIdAsync(Guid id)
        {
            return await _repo.GetFindingByIdAsync(id);
        }

        public async Task<ViewFinding> CreateFindingAsync(CreateFinding dto)
        {
            return await _repo.CreateFindingAsync(dto);
        }

        public async Task<ViewFinding?> UpdateFindingAsync(Guid id, UpdateFinding dto)
        {
            return await _repo.UpdateFindingAsync(id, dto);
        }

        public async Task<bool> DeleteFindingAsync(Guid id)
        {
            return await _repo.DeleteFindingAsync(id);
        }
    }
}
