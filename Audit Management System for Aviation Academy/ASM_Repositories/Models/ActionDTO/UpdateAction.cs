using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.ActionDTO
{
    public class UpdateAction
    {
        public Guid FindingId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid? AssignedTo { get; set; }
        public int? AssignedDeptId { get; set; }
        public byte? ProgressPercent { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string ReviewFeedback { get; set; }
    }
}
