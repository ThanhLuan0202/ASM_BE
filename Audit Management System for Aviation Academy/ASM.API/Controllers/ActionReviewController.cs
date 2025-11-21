using ASM.API.Helper;
using ASM.API.Hubs;
using ASM_Repositories.Models.ActionDTO;
using ASM_Repositories.Models.AttachmentDTO;
using ASM_Services.Interfaces;
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
    public class ActionReviewController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IActionService _actionService;
        private readonly NotificationHelper _notificationHelper;
        public ActionReviewController(IAttachmentService attachmentService, IHubContext<NotificationHub> hubContext, IActionService actionService, NotificationHelper notificationHelper)
        {
            _attachmentService = attachmentService;
            _actionService = actionService;
            _notificationHelper = notificationHelper;
        }

        [HttpPost("{actionId:guid}/approve")]
        public async Task<IActionResult> Approve(Guid actionId, [FromBody] CreateReviewFeedback request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Unauthorized("User not authenticated");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                    return Unauthorized("User ID not found in token");

                Guid userId = Guid.Parse(userIdClaim);

                var notif = await _actionService.ActionApprovedAsync(actionId, userId, request.Feedback);
                await _notificationHelper.SendToUserAsync(notif.UserId.ToString(), notif);
                return Ok(new
                {
                    Message = $"Action {actionId} approve + notification sent",
                    NotificationId = notif.NotificationId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("{actionId:guid}/returned")]
        public async Task<IActionResult> Reject(Guid actionId, [FromBody] CreateReviewFeedback request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Unauthorized("User not authenticated");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                    return Unauthorized("User ID not found in token");

                Guid userId = Guid.Parse(userIdClaim);

                var notif = await _actionService.ActionRejectedAsync(actionId ,userId, request.Feedback);

                await _notificationHelper.SendToUserAsync(notif.UserId.ToString(), notif);

                return Ok(new
                {
                    Message = $"Action {actionId} returned + notification sent",
                    NotificationId = notif.NotificationId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpPut("{actionId:guid}/approve/higher-level")]
        public async Task<IActionResult> ApproveByHigherLevel(Guid actionId, [FromBody] CreateReviewFeedback request)
        {
            try
            {
                await _actionService.ApproveByHigherLevel(actionId, request.Feedback);
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

        [HttpPut("{actionId:guid}/reject/higher-level")]
        public async Task<IActionResult> RejectByHigherLevel(Guid actionId, [FromBody] CreateReviewFeedback request)
        {
            try
            {
                await _actionService.RejectByHigherLevel(actionId, request.Feedback);
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
