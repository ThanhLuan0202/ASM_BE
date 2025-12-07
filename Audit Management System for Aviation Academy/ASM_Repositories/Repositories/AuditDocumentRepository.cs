using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
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

        public async Task<List<ViewAuditDocument?>> GetAuditDocumentByAuditIdAsync(Guid auditId)
        {
            var entities = await _context.AuditDocuments
                .Where(d => d.AuditId == auditId).ToListAsync(); ;
            return entities.Count == 0 ? new List<ViewAuditDocument?>()
                           : _mapper.Map<List<ViewAuditDocument?>>(entities);
        }

        public async Task AddAsync(AuditDocument doc)
        {
            _context.AuditDocuments.Add(doc);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusToArchivedAsync(Guid auditId)
        {
            if (auditId == Guid.Empty)
                throw new ArgumentException("AuditId cannot be empty.");

            var entities = await _context.AuditDocuments
                .Where(a => a.AuditId == auditId)
                .ToListAsync();

            if (!entities.Any())
                throw new InvalidOperationException($"No AuditDocument found for AuditId '{auditId}'.");

            foreach (var entity in entities)
            {
                entity.Status = "Archived";
                _context.Entry(entity).Property(x => x.Status).IsModified = true;
            }

            await _context.SaveChangesAsync();
        }

    }
}
