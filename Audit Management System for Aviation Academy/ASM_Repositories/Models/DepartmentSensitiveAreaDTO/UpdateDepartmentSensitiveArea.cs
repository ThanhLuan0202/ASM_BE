using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASM_Repositories.Models.DepartmentSensitiveAreaDTO
{
    public class UpdateDepartmentSensitiveArea
    {
        public List<string> SensitiveAreas { get; set; }

        [MaxLength(500, ErrorMessage = "DefaultNotes cannot exceed 500 characters")]
        public string DefaultNotes { get; set; }
    }
}

