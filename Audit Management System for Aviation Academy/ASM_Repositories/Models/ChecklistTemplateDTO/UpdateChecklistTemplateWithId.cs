using System;

namespace ASM_Repositories.Models.ChecklistTemplateDTO
{
    /// <summary>
    /// DTO để update ChecklistTemplate với TemplateId
    /// </summary>
    public class UpdateChecklistTemplateWithId
    {
        /// <summary>
        /// TemplateId của ChecklistTemplate cần update
        /// </summary>
        public Guid TemplateId { get; set; }

        /// <summary>
        /// Thông tin cần update cho ChecklistTemplate
        /// </summary>
        public UpdateChecklistTemplate Template { get; set; }
    }
}

