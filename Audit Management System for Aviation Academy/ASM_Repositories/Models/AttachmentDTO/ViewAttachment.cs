using System;

namespace ASM_Repositories.Models.AttachmentDTO
{
    public class ViewAttachment
    {
        public Guid AttachmentId { get; set; }
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public string FileName { get; set; }
        public string BlobPath { get; set; }
        public string ContentType { get; set; }
        public long? SizeBytes { get; set; }
        public Guid? UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
        public byte[] ContentHash { get; set; }
        public DateOnly? RetentionUntil { get; set; }
        public string Status { get; set; }
        public bool IsArchived { get; set; }
    }
}

