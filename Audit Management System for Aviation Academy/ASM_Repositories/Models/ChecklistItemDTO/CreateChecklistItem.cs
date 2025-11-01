using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.ChecklistItemDTO
{
    public class CreateChecklistItem
    {
        [Required(ErrorMessage = "TemplateId is required")]
        public Guid TemplateId { get; set; }

        [MaxLength(200, ErrorMessage = "Section cannot exceed 200 characters")]
        public string Section { get; set; }

        [Required(ErrorMessage = "Order is required")]
        public int Order { get; set; }

        [Required(ErrorMessage = "QuestionText is required")]
        public string QuestionText { get; set; }

        [MaxLength(50, ErrorMessage = "AnswerType cannot exceed 50 characters")]
        public string AnswerType { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }

        [MaxLength(20, ErrorMessage = "SeverityDefault cannot exceed 20 characters")]
        public string SeverityDefault { get; set; }
    }
}
