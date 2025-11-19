using ASM_Repositories.Models.ReportRequestDTO;
using ASM_Services.Interfaces;
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
        private readonly IReportRequestService _reportRequestService;
        public AuditReportsController(IAuditService auditService, IReportRequestService reportRequestService)
        {
            _auditService = auditService;
            _reportRequestService = reportRequestService;
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

        [HttpGet("Note/{auditId:guid}")]
        public async Task<IActionResult> GetNoteByAuditId(Guid auditId)
        {
            // Gọi hàm service/repo
            var note = await _reportRequestService.GetNoteByAuditIdAsync(auditId);

            if (note == null)
                return NotFound(new { Message = "ReportRequest not found for this auditId." });

            return Ok(new { Reason = note });
        }
    }
}
