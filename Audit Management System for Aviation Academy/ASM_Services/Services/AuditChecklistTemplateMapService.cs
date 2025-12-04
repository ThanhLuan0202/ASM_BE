using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditChecklistTemplateMapDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditChecklistTemplateMapService : IAuditChecklistTemplateMapService
    {
        private readonly IAuditChecklistTemplateMapRepository _repo;

        public AuditChecklistTemplateMapService(IAuditChecklistTemplateMapRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<ViewAuditChecklistTemplateMap>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<ViewAuditChecklistTemplateMap> GetAsync(Guid auditId, Guid templateId)
            => _repo.GetAsync(auditId, templateId);

        public Task<ViewAuditChecklistTemplateMap> CreateAsync(CreateAuditChecklistTemplateMap dto)
            => _repo.CreateAsync(dto);

        public Task<ViewAuditChecklistTemplateMap> UpdateAsync(Guid auditId, Guid templateId, UpdateAuditChecklistTemplateMap dto)
            => _repo.UpdateAsync(auditId, templateId, dto);

        public Task DeleteAsync(Guid auditId, Guid templateId)
            => _repo.DeleteAsync(auditId, templateId);
    }
}
