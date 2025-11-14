using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_Services.Interfaces
{
    public interface IPdfGeneratorService
    {
        byte[] GeneratePdf(ViewAuditSummary summary, List<Finding> findings, List<Attachment> attachments, byte[]? logo, List<byte[]> charts);
    }
}
