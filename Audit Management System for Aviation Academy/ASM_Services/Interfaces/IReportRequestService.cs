using ASM_Repositories.Models.ReportRequestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IReportRequestService
    {
        Task<IEnumerable<ViewReportRequest>> GetAllAsync();
        Task<ViewReportRequest?> GetByIdAsync(Guid id);
        Task<ViewReportRequest> CreateAsync(CreateReportRequest dto, Guid userId);
        Task<ViewReportRequest?> UpdateAsync(Guid id, UpdateReportRequest dto, Guid userId);
        Task<bool> SoftDeleteAsync(Guid id, Guid userId);
        Task<string?> GetNoteByAuditIdAsync(Guid auditId);
    }
}