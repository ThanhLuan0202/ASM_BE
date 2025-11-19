using ASM_Repositories.Models.ReportRequestDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditReportsController : ControllerBase
    {
        private readonly IAuditService _auditService;
        public AuditReportsController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpPost("{auditId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid auditId)
        {
            try
            {
                await _auditService.UpdateReportStatusAsync(auditId, "Completed", "Approved");
                return Ok(new { message = $"Audit {auditId} approved successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{auditId:guid}/reject")]
        public async Task<IActionResult> Reject(Guid auditId, [FromBody] CreateReasonRejectReportRequest request)
        {
            try
            {
                await _auditService.UpdateReportStatusAndNoteAsync(auditId, "Returned", "Rejected", request.Reason);
                return Ok(new { message = $"Audit {auditId} rejected successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
