using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditCriterionDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditCriterionService : IAuditCriterionService
    {
        private readonly IAuditCriterionRepository _repo;

        public AuditCriterionService(IAuditCriterionRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditCriterion>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewAuditCriterion?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<ViewAuditCriterion> CreateAsync(CreateAuditCriterion dto) => _repo.CreateAsync(dto);
        public Task<ViewAuditCriterion?> UpdateAsync(Guid id, UpdateAuditCriterion dto) => _repo.UpdateAsync(id, dto);
        public Task<bool> SoftDeleteAsync(Guid id) => _repo.SoftDeleteAsync(id);
    }
}
