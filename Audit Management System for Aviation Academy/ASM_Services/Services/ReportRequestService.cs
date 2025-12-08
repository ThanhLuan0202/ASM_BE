using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ReportRequestDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class ReportRequestService : IReportRequestService
    {
        private readonly IReportRequestRepository _repo;
        private readonly IAuditLogService _logService;

        public ReportRequestService(IReportRequestRepository repo, IAuditLogService logService)
        {
            _repo = repo;
            _logService = logService;
        }

        public Task<IEnumerable<ViewReportRequest>> GetAllAsync() => _repo.GetAllAsync();
        public Task<ViewReportRequest?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public async Task<ViewReportRequest> CreateAsync(CreateReportRequest dto, Guid userId)
        {
            var created = await _repo.CreateAsync(dto);
            await _logService.LogCreateAsync(created, created.ReportRequestId, userId, "ReportRequest");
            return created;
        }
        public async Task<ViewReportRequest?> UpdateAsync(Guid id, UpdateReportRequest dto, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var updated = await _repo.UpdateAsync(id, dto);
            if (before != null && updated != null)
            {
                await _logService.LogUpdateAsync(before, updated, id, userId, "ReportRequest");
            }
            return updated;
        }
        public async Task<bool> SoftDeleteAsync(Guid id, Guid userId)
        {
            var before = await _repo.GetByIdAsync(id);
            var success = await _repo.SoftDeleteAsync(id);
            if (success && before != null)
            {
                await _logService.LogDeleteAsync(before, id, userId, "ReportRequest");
            }
            return success;
        }
        public Task<string?> GetNoteByAuditIdAsync(Guid auditId) => _repo.GetNoteByAuditIdAsync(auditId);
    }
}
