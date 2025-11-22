using ASM.API.Helper;
using ASM_Repositories.Models.ReportRequestDTO;
using ASM_Services.Interfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using ASM_Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditReportsController : ControllerBase
    {
        private readonly IAuditService _auditService;
        private readonly IReportRequestService _reportRequestService;
        private readonly NotificationHelper _notificationHelper;
        public AuditReportsController(IAuditService auditService, IReportRequestService reportRequestService, NotificationHelper notificationHelper)
        {
            _auditService = auditService;
            _reportRequestService = reportRequestService;
            _notificationHelper = notificationHelper;
        }

        [HttpPost("{auditId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid auditId, [FromBody] CreateNoteReportRequest request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Unauthorized("User not authenticated");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                    return Unauthorized("User ID not found in token");

                Guid userId = Guid.Parse(userIdClaim);
                if (request == null) return BadRequest("Invalid request.");

                var notif = await _auditService.ReportApproveAsync(auditId, userId, request.Note);

                var sentSuccess = new List<object>();
                var sentFailed = new List<object>();

                try
                {
                    await _notificationHelper.SendToUserAsync(notif.UserId.ToString(), notif);
                    sentSuccess.Add(new { UserId = notif.UserId, NotificationId = notif.NotificationId });
                }
                catch (Exception ex)
                {
                    sentFailed.Add(new { UserId = notif.UserId, NotificationId = notif.NotificationId, Error = ex.Message });
                }

                return Ok(new
                {
                    Message = $"Audit {auditId} approved successfully.",
                    SentSuccess = sentSuccess,
                    SentFailed = sentFailed
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpPut("{auditId:guid}/reject")]
        public async Task<IActionResult> Reject(Guid auditId, [FromBody] CreateNoteReportRequest request)
        {
            try
            {
                await _auditService.UpdateReportStatusAndNoteAsync(auditId, "Returned", "Rejected", request.Note);
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
            var note = await _reportRequestService.GetNoteByAuditIdAsync(auditId);

            if (note == null)
                return NotFound(new { Message = "ReportRequest not found for this auditId." });

            return Ok(new { Reason = note });
        }
    }
}
