using System;

namespace ASM_Repositories.Models.DepartmentHeadDTO
{
    public class ViewDepartmentHead
    {
        public Guid DeptHeadId { get; set; }
        public int DeptId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; }
    }
}

