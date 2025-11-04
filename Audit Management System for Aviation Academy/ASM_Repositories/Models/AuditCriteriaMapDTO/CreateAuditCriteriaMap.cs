using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AuditCriteriaMapDTO
{
    public class CreateAuditCriteriaMap
    {
        [Required(ErrorMessage = "AuditId is required")]
        public Guid AuditId { get; set; }

        [Required(ErrorMessage = "CriteriaId is required")]
        public Guid CriteriaId { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }
    }
}
