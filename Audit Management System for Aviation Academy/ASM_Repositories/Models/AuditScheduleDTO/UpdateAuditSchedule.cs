using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.AuditScheduleDTO
{
    public class UpdateAuditSchedule
    {
        [Required(ErrorMessage = "MilestoneName is required")]
        [MaxLength(50, ErrorMessage = "MilestoneName cannot exceed 50 characters")]
        public string MilestoneName { get; set; }

        [Required(ErrorMessage = "DueDate is required")]
        public DateTime DueDate { get; set; }

        [MaxLength(255, ErrorMessage = "Notes cannot exceed 255 characters")]
        public string Notes { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }
    }
}

