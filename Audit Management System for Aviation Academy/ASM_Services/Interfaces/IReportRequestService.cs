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
        Task<ViewReportRequest> CreateAsync(CreateReportRequest dto);
        Task<ViewReportRequest?> UpdateAsync(Guid id, UpdateReportRequest dto);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<string?> GetNoteByAuditIdAsync(Guid auditId);
    }
}