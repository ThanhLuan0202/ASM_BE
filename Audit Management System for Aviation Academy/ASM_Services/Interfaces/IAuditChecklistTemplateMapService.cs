using ASM_Repositories.Models.AuditChecklistTemplateMapDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IAuditChecklistTemplateMapService
    {
        Task<IEnumerable<ViewAuditChecklistTemplateMap>> GetAllAsync();
        Task<ViewAuditChecklistTemplateMap> GetAsync(Guid auditId, Guid templateId);
        Task<ViewAuditChecklistTemplateMap> CreateAsync(CreateAuditChecklistTemplateMap dto, Guid userId);
        Task<ViewAuditChecklistTemplateMap> UpdateAsync(Guid auditId, Guid templateId, UpdateAuditChecklistTemplateMap dto, Guid userId);
        Task DeleteAsync(Guid auditId, Guid templateId, Guid userId);
    }
}
