using System;

namespace ASM_Repositories.Models.DepartmentSensitiveAreaDTO
{
    public class ViewDepartmentSensitiveArea
    {
        public Guid Id { get; set; }
        public int DeptId { get; set; }
        public string SensitiveArea { get; set; }
        public string Level { get; set; }
        public string DefaultNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        
        // Navigation properties
        public string DepartmentName { get; set; }
        public string LevelName { get; set; }
        public string CreatedByName { get; set; }
    }
}

