using System;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.DepartmentHeadDTO
{
    public class CreateDepartmentHead
    {
        [Required(ErrorMessage = "DeptId is required")]
        public int DeptId { get; set; }

        [Required(ErrorMessage = "UserId is required")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "StartDate is required")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; }
    }
}

