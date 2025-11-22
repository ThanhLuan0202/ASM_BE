using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.ReportRequestDTO;
using ASM_Repositories.Models.RoleDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class ReportRequestRepository : IReportRequestRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public ReportRequestRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewReportRequest>> GetAllAsync()
        {
            var data = await _context.ReportRequests.ToListAsync();
            return _mapper.Map<IEnumerable<ViewReportRequest>>(data);
        }

        public async Task<ViewReportRequest?> GetByIdAsync(Guid id)
        {
            var entity = await _context.ReportRequests.FindAsync(id);
            return _mapper.Map<ViewReportRequest?>(entity);
        }

        public async Task<ViewReportRequest> CreateAsync(CreateReportRequest dto)
        {
            if (dto.RequestedBy.HasValue &&
                !await _context.UserAccounts.AnyAsync(u => u.UserId == dto.RequestedBy))
                throw new ArgumentException($"RequestedBy '{dto.RequestedBy}' does not exist.");

            var entity = _mapper.Map<ReportRequest>(dto); 

            _context.ReportRequests.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewReportRequest>(entity);
        }

        public async Task<ViewReportRequest?> UpdateAsync(Guid id, UpdateReportRequest dto)
        {
            var entity = await _context.ReportRequests.FindAsync(id);
            if (entity == null) return null;

            _mapper.Map(dto, entity);

            await _context.SaveChangesAsync();
            return _mapper.Map<ViewReportRequest>(entity);
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var entity = await _context.ReportRequests.FirstOrDefaultAsync(x => x.ReportRequestId == id);
            if (entity == null || entity.Status == "Inactive") return false;

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }

        public Task AddReportRequestAsync(ReportRequest rr)
        {
            _context.ReportRequests.Add(rr);
            return Task.CompletedTask;
        }

        public async Task<ReportRequest?> UpdateStatusByAuditIdAsync(Guid auditId, string status)
        {
            var rr = await _context.ReportRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Parameters.Contains($"\"auditId\":\"{auditId}\""));

            if (rr != null)
            {
                _context.ReportRequests.Attach(rr);
                rr.Status = status;
                rr.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return rr;
        }

        public async Task<ReportRequest?> UpdateStatusAndNoteByAuditIdAsync(Guid auditId, string status, string note)
        {
            var rr = await _context.ReportRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Parameters.Contains($"\"auditId\":\"{auditId}\""));

            if (rr != null)
            {
                _context.ReportRequests.Attach(rr);
                rr.Status = status;
                rr.Note = note;
                rr.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return rr;
        }

        public async Task<string?> GetNoteByAuditIdAsync(Guid auditId)
        {
            var rr = await _context.ReportRequests
                .FirstOrDefaultAsync(r => r.Parameters.Contains($"\"auditId\":\"{auditId}\""));

            return rr?.Note;
        }

        public async Task<ReportRequest> CreateReportRequestAsync(Guid auditId, string pdfUrl, Guid requestedBy)
        {
            var rr = new ReportRequest
            {
                ReportRequestId = Guid.NewGuid(),
                RequestedBy = requestedBy,
                Parameters = $"{{\"auditId\":\"{auditId}\"}}",
                Status = "Pending",
                FilePath = pdfUrl,
                RequestedAt = DateTime.UtcNow
            };

            _context.ReportRequests.Add(rr);
            await _context.SaveChangesAsync();

            return rr;
        }
    }

}
