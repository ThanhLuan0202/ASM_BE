using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.AuditDocumentDTO
{
    public class ViewAuditDocument
    {
        public Guid DocId { get; set; }

        public Guid AuditId { get; set; }

        public string DocumentType { get; set; }

        public string Title { get; set; }

        public string BlobPath { get; set; }

        public string ContentType { get; set; }

        public long? SizeBytes { get; set; }

        public Guid? UploadedBy { get; set; }

        public DateTime UploadedAt { get; set; }

        public bool IsFinalVersion { get; set; }

        public string Status { get; set; }

    }
}
