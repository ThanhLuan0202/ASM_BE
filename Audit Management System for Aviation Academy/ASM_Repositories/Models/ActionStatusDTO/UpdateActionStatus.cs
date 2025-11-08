using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.ActionStatusDTO
{
    public class UpdateActionStatus
    {
        [Required(ErrorMessage = "ActionStatus is required")]
        [MaxLength(50, ErrorMessage = "ActionStatus cannot exceed 50 characters")]
        public string ActionStatus1 { get; set; }
    }
}

