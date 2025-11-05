using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Repositories.Models.ReportRequestDTO
{
    public class UpdateReportRequest
    {
        public string Parameters { get; set; }
        public string Status { get; set; }
        public string FilePath { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

}
