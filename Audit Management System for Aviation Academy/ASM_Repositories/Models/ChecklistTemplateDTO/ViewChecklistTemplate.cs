using System;

namespace ASM_Repositories.Models.ChecklistTemplateDTO
{
    public class ViewChecklistTemplate
    {
        public Guid TemplateId { get; set; }

        public string Name { get; set; }

        public string Version { get; set; }

        public string Description { get; set; }

        public Guid? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? DeptId { get; set; }

        public string Status { get; set; }

        public bool IsActive { get; set; }
    }
}
