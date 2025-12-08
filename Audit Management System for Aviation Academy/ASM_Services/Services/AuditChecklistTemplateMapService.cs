using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditChecklistTemplateMapDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
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
        private readonly IAuditLogService _logService;

        public AuditChecklistTemplateMapService(IAuditChecklistTemplateMapRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewAuditChecklistTemplateMap>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<ViewAuditChecklistTemplateMap> GetAsync(Guid auditId, Guid templateId)
            => _repo.GetAsync(auditId, templateId);

        public async Task<ViewAuditChecklistTemplateMap> CreateAsync(CreateAuditChecklistTemplateMap dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            await _logService.LogCreateAsync(created, created.AuditId, userId, "AuditChecklistTemplateMap");
            return created;
        }

        public async Task<ViewAuditChecklistTemplateMap> UpdateAsync(Guid auditId, Guid templateId, UpdateAuditChecklistTemplateMap dto, Guid userId)
        {
            var before = await _repo.GetAsync(auditId, templateId);
            var updated = await _repo.UpdateAsync(auditId, templateId, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, auditId, userId, "AuditChecklistTemplateMap");
            }
            return updated;
        }

        public async Task DeleteAsync(Guid auditId, Guid templateId, Guid userId)
        {
            var before = await _repo.GetAsync(auditId, templateId);
            await _repo.DeleteAsync(auditId, templateId);
            if (before != null)
            {
                await _logService.LogDeleteAsync(before, auditId, userId, "AuditChecklistTemplateMap");
            }
        }
    }
}
