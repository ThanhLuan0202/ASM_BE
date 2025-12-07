using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.ChecklistItemNoFindingDTO
{
    public class ViewChecklistItemNoFinding
    {
        public int Id { get; set; }
        public Guid AuditChecklistItemId { get; set; }
        public string Title { get; set; }
        public string Reason { get; set; }
        public DateOnly? DateOfCompliance { get; set; }
        public TimeOnly? TimeOfCompliance { get; set; }
        public string Department { get; set; }
        public Guid WitnessId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
