using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Migrations;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    internal class AuditDocumentRepository : IAuditDocumentRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;
        public AuditDocumentRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task AddAuditDocumentAsync(AuditDocument doc)
        {
            _context.AuditDocuments.Add(doc);
            return Task.CompletedTask;
        }
        public async Task<AuditDocument?> UpdateStatusByAuditIdAsync(Guid auditId, string status)
        {
            // Load entity nhưng không tracking
            var doc = await _context.AuditDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.AuditId == auditId);

            if (doc != null)
            {
                // Attach entity vào context hiện tại
                _context.AuditDocuments.Attach(doc);

                // Cập nhật giá trị
                doc.Status = status;
                doc.UploadedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return doc;
        }

    }
}
