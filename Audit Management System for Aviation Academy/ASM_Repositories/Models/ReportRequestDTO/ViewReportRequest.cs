using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.ReportRequestDTO
{
    public class ViewReportRequest
    {
        public Guid ReportRequestId { get; set; }
        public Guid? RequestedBy { get; set; }
        public string Parameters { get; set; }
        public string Status { get; set; }
        public string FilePath { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
