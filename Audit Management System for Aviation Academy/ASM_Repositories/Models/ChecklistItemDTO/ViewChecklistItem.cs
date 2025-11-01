using System;

namespace ASM_Repositories.Models.ChecklistItemDTO
{
    public class ViewChecklistItem
    {
        public Guid ItemId { get; set; }

        public Guid TemplateId { get; set; }

        public string Section { get; set; }

        public int Order { get; set; }

        public string QuestionText { get; set; }

        public string AnswerType { get; set; }

        public string Status { get; set; }

        public string SeverityDefault { get; set; }
    }
}
