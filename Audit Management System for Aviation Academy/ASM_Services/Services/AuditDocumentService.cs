using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AuditDocumentDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Http;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Services
{
    public class AuditDocumentService : IAuditDocumentService
    {
        private readonly IAuditDocumentRepository _repo;
        private readonly IFirebaseUploadService _firebaseUploadService;
        private readonly IAuditRepository _auditRepo;
        public AuditDocumentService(IAuditDocumentRepository repo, IFirebaseUploadService firebaseUploadService, IAuditRepository auditRepo)
        {
            _repo = repo;
            _firebaseUploadService = firebaseUploadService;
            _auditRepo = auditRepo;
        }
        public async Task<List<ViewAuditDocument?>> GetAuditDocumentByAuditIdAsync(Guid auditId)
        {
            return await _repo.GetAuditDocumentByAuditIdAsync(auditId);
        }

        public async Task<AuditDocument?> UploadAndUpdateAuditDocumentAsync(Guid auditId, IFormFile file, Guid uploadedBy)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty", nameof(file));

            var blobPath = await _firebaseUploadService.UploadFileAsync(file, "AuditDocuments");

            var updatedDoc = await _repo.UpdateAsync(auditId, doc =>
            {
                doc.Status = "Completed";
                doc.BlobPath = blobPath;
                doc.ContentType = file.ContentType;
                doc.SizeBytes = file.Length;
                doc.UploadedBy = uploadedBy;
                doc.UploadedAt = DateTime.UtcNow;
                doc.IsFinalVersion = true;
            });

            return updatedDoc;
        }

        public async Task<List<AuditDocument>> UploadMultipleAsync(Guid auditId, List<IFormFile> files, Guid uploadedBy)
        {
            var audit = await _auditRepo.GetAuditByIdAsync(auditId);
            var uploadedDocs = new List<AuditDocument>();

            foreach (var file in files)
            {
                var blobPath = await _firebaseUploadService.UploadFileAsync(file, "AuditDocuments");

                var doc = new AuditDocument
                {
                    DocId = Guid.NewGuid(),
                    AuditId = auditId,
                    DocumentType = "Submitted Report",
                    Title = $"{audit?.Title} - Submitted Report",
                    BlobPath = blobPath,
                    ContentType = file.ContentType,
                    SizeBytes = file.Length,
                    UploadedBy = uploadedBy,
                    UploadedAt = DateTime.UtcNow,
                    IsFinalVersion = true,
                    Status = "Completed",
                };

                await _repo.AddAsync(doc);
                uploadedDocs.Add(doc);
            }

            return uploadedDocs;
        }

    }
}
