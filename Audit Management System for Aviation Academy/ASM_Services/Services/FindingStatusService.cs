using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.FindingStatusDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class FindingStatusService : IFindingStatusService
    {
        private readonly IFindingStatusRepository _repo;

        public FindingStatusService(IFindingStatusRepository repo)
        {
            _repo = repo;
        }

        public Task<List<ViewFindingStatus>> GetAllAsync() => _repo.GetAllAsync();

        public Task<ViewFindingStatus> GetByIdAsync(string status) => _repo.GetByIdAsync(status);

        public Task<ViewFindingStatus> CreateAsync(CreateFindingStatus dto) => _repo.AddAsync(dto);

        public Task<bool> UpdateAsync(string status, UpdateFindingStatus dto) => _repo.UpdateAsync(status, dto);

        public Task<bool> DeleteAsync(string status) => _repo.DeleteAsync(status);
    }

}
