using ASM_Repositories.Models.AuditChecklistTemplateMapDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Interfaces
{
    public interface IAuditChecklistTemplateMapRepository
    {
        Task<IEnumerable<ViewAuditChecklistTemplateMap>> GetAllAsync();
        Task<ViewAuditChecklistTemplateMap> GetAsync(Guid auditId, Guid templateId);
        Task<ViewAuditChecklistTemplateMap> CreateAsync(CreateAuditChecklistTemplateMap dto);
        Task<ViewAuditChecklistTemplateMap> UpdateAsync(Guid auditId, Guid templateId, UpdateAuditChecklistTemplateMap dto);
        Task DeleteAsync(Guid auditId, Guid templateId);
    }
}
