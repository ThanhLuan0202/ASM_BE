using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AuditChecklistItemDTO
{
    public class UpdateAuditChecklistItem
    {
        [Required(ErrorMessage = "QuestionTextSnapshot is required")]
        public string QuestionTextSnapshot { get; set; }

        [MaxLength(200, ErrorMessage = "Section cannot exceed 200 characters")]
        public string Section { get; set; }

        public int? Order { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string Comment { get; set; }
    }
}
