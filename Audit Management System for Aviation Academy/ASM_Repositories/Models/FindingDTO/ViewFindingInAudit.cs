using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.FindingDTO
{
    public class ViewFindingInAudit 
    {
        public int Month { get; set; }
        public int Total { get; set; }
        public int Open { get; set; }
        public int Closed { get; set; }
        public int Overdue { get; set; }

        public List<ViewFindingDetail> Findings { get; set; } = new();
    }
}
