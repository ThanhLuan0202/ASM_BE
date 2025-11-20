using ASM.API.Hubs;
using ASM_Repositories.Models.AttachmentDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentReviewController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IHubContext<NotificationHub> _hubContext;
        public AttachmentReviewController(IAttachmentService attachmentService, IHubContext<NotificationHub> hubContext)
        {
            _attachmentService = attachmentService;
            _hubContext = hubContext;
        }

        [HttpPut("{attachmentId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid attachmentId)
        {
            try
            {
                await _attachmentService.UpdateAttachmentStatusAsync(attachmentId, "Approved");
                return Ok(new { message = $"Attachment {attachmentId} approved successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("{attachmentId:guid}/returned")]
        public async Task<IActionResult> Reject(Guid attachmentId, [FromBody] CreateReasonRejectAttachment request)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("User not authenticated");

                Guid userId = Guid.Parse(userIdClaim);

                var notif = await _attachmentService.AttachmentRejectedAsync(
                    attachmentId,
                    userId,
                    request.Reason
                );

                await _hubContext.Clients.All.SendAsync("ReceiveNotification", new
                {
                    notif.NotificationId,
                    notif.Title,
                    notif.Message,
                    notif.CreatedAt,
                    notif.EntityId
                });

                return Ok(new
                {
                    Message = $"Attachment {attachmentId} returned + notification sent",
                    NotificationId = notif.NotificationId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpPut("{attachmentId:guid}/approve/higher-level")]
        public async Task<IActionResult> ApproveByHigherLevel(Guid attachmentId)
        {
            try
            {
                await _attachmentService.ApproveByHigherLevel(attachmentId);
                return Ok(new { message = "Attachment, Action, and Finding approved successfully." });
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

        [HttpPut("{attachmentId:guid}/reject/higher-level")]
        public async Task<IActionResult> RejectByHigherLevel(Guid attachmentId)
        {
            try
            {
                await _attachmentService.RejectByHigherLevel(attachmentId);
                return Ok(new { message = "Attachment, Action, and Finding rejected successfully." });
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

    }
}
