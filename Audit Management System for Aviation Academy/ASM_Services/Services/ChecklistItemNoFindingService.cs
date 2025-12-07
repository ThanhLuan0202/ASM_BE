using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ChecklistItemNoFindingDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class ChecklistItemNoFindingService : IChecklistItemNoFindingService
    {
        private readonly IChecklistItemNoFindingRepository _repo;

        public ChecklistItemNoFindingService(IChecklistItemNoFindingRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewChecklistItemNoFinding>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewChecklistItemNoFinding?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task<ViewChecklistItemNoFinding> CreateAsync(CreateChecklistItemNoFinding dto) => _repo.CreateAsync(dto);
        public Task<ViewChecklistItemNoFinding?> UpdateAsync(int id, UpdateChecklistItemNoFinding dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }
}
