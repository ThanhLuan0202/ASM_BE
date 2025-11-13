using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AttachmentDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AttachmentRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAttachment>> GetAllAsync()
        {
            var data = await _context.Attachments
                .Include(x => x.EntityTypeNavigation)
                .Include(x => x.UploadedByNavigation)
                .OrderByDescending(x => x.UploadedAt)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ViewAttachment>>(data);
        }

        public async Task<ViewAttachment?> GetByIdAsync(Guid attachmentId)
        {
            var entity = await _context.Attachments
                .Include(x => x.EntityTypeNavigation)
                .Include(x => x.UploadedByNavigation)
                .FirstOrDefaultAsync(x => x.AttachmentId == attachmentId);
            return entity == null ? null : _mapper.Map<ViewAttachment>(entity);
        }

        public async Task<IEnumerable<ViewAttachment>> GetByEntityAsync(string entityType, Guid entityId)
        {
            var data = await _context.Attachments
                .Include(x => x.EntityTypeNavigation)
                .Include(x => x.UploadedByNavigation)
                .Where(x => x.EntityType == entityType && x.EntityId == entityId)
                .OrderByDescending(x => x.UploadedAt)
                .ToListAsync();
            return _mapper.Map<IEnumerable<ViewAttachment>>(data);
        }

        public async Task<ViewAttachment> CreateAsync(CreateAttachment dto, string fileName, string blobPath, string contentType, long sizeBytes, Guid? uploadedBy)
        {
            var entityTypeExists = await _context.AttachmentEntityTypes.AnyAsync(e => e.EntityType == dto.EntityType);
            if (!entityTypeExists)
                throw new InvalidOperationException($"EntityType '{dto.EntityType}' does not exist.");

            if (uploadedBy.HasValue)
            {
                var userExists = await _context.UserAccounts.AnyAsync(u => u.UserId == uploadedBy.Value);
                if (!userExists)
                    throw new InvalidOperationException($"User with ID {uploadedBy.Value} does not exist.");
            }

            var entity = _mapper.Map<Attachment>(dto);
            entity.AttachmentId = Guid.NewGuid();
            entity.FileName = fileName;
            entity.BlobPath = blobPath;
            entity.ContentType = contentType;
            entity.SizeBytes = sizeBytes;
            entity.UploadedBy = uploadedBy;
            entity.UploadedAt = DateTime.UtcNow;
            
            if (string.IsNullOrWhiteSpace(entity.Status))
                entity.Status = "Active";

            _context.Attachments.Add(entity);
            await _context.SaveChangesAsync();

            var created = await _context.Attachments
                .Include(x => x.EntityTypeNavigation)
                .Include(x => x.UploadedByNavigation)
                .FirstOrDefaultAsync(x => x.AttachmentId == entity.AttachmentId);

            return _mapper.Map<ViewAttachment>(created);
        }

        public async Task<ViewAttachment?> UpdateAsync(Guid attachmentId, UpdateAttachment dto)
        {
            var entity = await _context.Attachments
                .FirstOrDefaultAsync(x => x.AttachmentId == attachmentId);

            if (entity == null) return null;

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            var updated = await _context.Attachments
                .Include(x => x.EntityTypeNavigation)
                .Include(x => x.UploadedByNavigation)
                .FirstOrDefaultAsync(x => x.AttachmentId == attachmentId);

            return _mapper.Map<ViewAttachment>(updated);
        }

        public async Task<ViewAttachment?> UpdateFileAsync(Guid attachmentId, string fileName, string blobPath, string contentType, long sizeBytes)
        {
            var entity = await _context.Attachments
                .FirstOrDefaultAsync(x => x.AttachmentId == attachmentId);

            if (entity == null) return null;

            entity.FileName = fileName;
            entity.BlobPath = blobPath;
            entity.ContentType = contentType;
            entity.SizeBytes = sizeBytes;
            entity.UploadedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var updated = await _context.Attachments
                .Include(x => x.EntityTypeNavigation)
                .Include(x => x.UploadedByNavigation)
                .FirstOrDefaultAsync(x => x.AttachmentId == attachmentId);

            return _mapper.Map<ViewAttachment>(updated);
        }

        public async Task<bool> DeleteAsync(Guid attachmentId)
        {
            var entity = await _context.Attachments.FindAsync(attachmentId);
            if (entity == null) return false;

            entity.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

