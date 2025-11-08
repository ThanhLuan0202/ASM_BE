using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AuditStatusDTO
{
    public class CreateAuditStatus
    {
        [Required(ErrorMessage = "AuditStatus is required")]
        [MaxLength(50, ErrorMessage = "AuditStatus cannot exceed 50 characters")]
        public string AuditStatus1 { get; set; }
    }
}

