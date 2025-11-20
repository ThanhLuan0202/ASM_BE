using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Migrations;
using ASM_Repositories.Models.AuditCriterionDTO;
using ASM_Repositories.Models.AuditDocumentDTO;
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
            var doc = await _context.AuditDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.AuditId == auditId);

            if (doc != null)
            {
                _context.AuditDocuments.Attach(doc);

                doc.Status = status;
                doc.UploadedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return doc;
        }

        public async Task<AuditDocument?> UpdateAsync(Guid auditId, Action<AuditDocument> updateAction)
        {
            var doc = await _context.AuditDocuments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.AuditId == auditId);

            if (doc == null) return null;

            updateAction(doc);
            _context.AuditDocuments.Update(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<ViewAuditDocument?> GetAuditDocumentByAuditIdAsync(Guid auditId)
        {
            var entity = await _context.AuditDocuments
                .FirstOrDefaultAsync(d => d.AuditId == auditId);
            return entity == null ? null : _mapper.Map<ViewAuditDocument>(entity);
        }

        public async Task AddAsync(AuditDocument doc)
        {
            _context.AuditDocuments.Add(doc);
            await _context.SaveChangesAsync();
        }
    }
}
