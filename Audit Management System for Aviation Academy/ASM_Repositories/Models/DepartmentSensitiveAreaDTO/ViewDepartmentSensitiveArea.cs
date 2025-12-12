using System;
using System.Collections.Generic;

namespace ASM_Repositories.Models.DepartmentSensitiveAreaDTO
{
    public class ViewDepartmentSensitiveArea
    {
        public Guid Id { get; set; }
        public int DeptId { get; set; }
        public List<string> SensitiveAreas { get; set; }
        public string DefaultNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        
        // Navigation properties
        public string DepartmentName { get; set; }
    }
}

