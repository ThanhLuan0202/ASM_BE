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

        [HttpPost("{actionId:guid}/verified")]
        public async Task<IActionResult> Verified(Guid actionId, [FromBody] CreateReviewFeedback request)
        {
            try
            {
                if (actionId == Guid.Empty)
                    return BadRequest(new { message = "Invalid ActionId" });

                var updated = await _actionService.ActionVerifiedAsync(actionId, request.Feedback);
                if (!updated)
                    return NotFound(new { message = "Action not found or inactive." });

                return Ok(new { message = "Action status updated to Verified." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("{actionId:guid}/declined")]
        public async Task<IActionResult> Declined(Guid actionId, [FromBody] CreateReviewFeedback request)
        {
            try
            {
                if (actionId == Guid.Empty)
                    return BadRequest(new { message = "Invalid ActionId" });

                var updated = await _actionService.ActionDeclinedAsync(actionId, request.Feedback);
                if (!updated)
                    return NotFound(new { message = "Action not found or inactive." });

                return Ok(new { message = "Action status updated to Declined." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
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

                var notifications = await _actionService.ActionApprovedAsync(actionId, userId, request.Feedback);

                if (notifications.Count != 2)
                    return BadRequest("Expected 2 notifications from service");

                var notif1 = notifications[0]; 
                var notif2 = notifications[1];

                var sentSuccess = new List<object>();
                var sentFailed = new List<object>();

                try
                {
                    await _notificationHelper.SendToUserAsync(notif1.UserId.ToString(), notif1);
                    sentSuccess.Add(new { UserId = notif1.UserId, NotificationId = notif1.NotificationId });
                }
                catch (Exception ex)
                {
                    sentFailed.Add(new { UserId = notif1.UserId, NotificationId = notif1.NotificationId, Error = ex.Message });
                }

                try
                {
                    await _notificationHelper.SendToUserAsync(notif2.UserId.ToString(), notif2);
                    sentSuccess.Add(new { UserId = notif2.UserId, NotificationId = notif2.NotificationId });
                }
                catch (Exception ex)
                {
                    sentFailed.Add(new { UserId = notif2.UserId, NotificationId = notif2.NotificationId, Error = ex.Message });
                }

                return Ok(new
                {
                    Message = $"Action {actionId} approved and notifications sent",
                    SentSuccess = sentSuccess,
                    SentFailed = sentFailed
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
                if (!User.Identity.IsAuthenticated)
                    return Unauthorized("User not authenticated");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                    return Unauthorized("User ID not found in token");

                Guid userId = Guid.Parse(userIdClaim);

                var notifications = await _actionService.ApproveByHigherLevel(actionId, userId, request.Feedback);

                if (notifications.Count != 2)
                    return StatusCode(500, new { error = "Expected 2 notifications from service" });

                var notif1 = notifications[0];   
                var notif2 = notifications[1];

                var sentSuccess = new List<object>();
                var sentFailed = new List<object>();

                try
                {
                    await _notificationHelper.SendToUserAsync(notif1.UserId.ToString(), notif1);
                    sentSuccess.Add(new { UserId = notif1.UserId, NotificationId = notif1.NotificationId });
                }
                catch (Exception ex)
                {
                    sentFailed.Add(new { UserId = notif1.UserId, NotificationId = notif1.NotificationId, Error = ex.Message });
                }

                try
                {
                    await _notificationHelper.SendToUserAsync(notif2.UserId.ToString(), notif2);
                    sentSuccess.Add(new {  UserId = notif2.UserId, NotificationId = notif2.NotificationId });
                }
                catch (Exception ex)
                {
                    sentFailed.Add(new { UserId = notif2.UserId, NotificationId = notif2.NotificationId, Error = ex.Message });
                }

                return Ok(new
                {
                    Message = "Attachment, Action approved and Finding closed successfully.",
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

        [HttpPut("{actionId:guid}/reject/higher-level")]
        public async Task<IActionResult> RejectByHigherLevel(Guid actionId, [FromBody] CreateReviewFeedback request)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Unauthorized("User not authenticated");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                    return Unauthorized("User ID not found in token");

                Guid userId = Guid.Parse(userIdClaim);

                var notifications = await _actionService.RejectByHigherLevel(actionId, userId, request.Feedback);

                if (notifications.Count != 2)
                    return StatusCode(500, new { error = "Expected 2 notifications from service" });

                var notif1 = notifications[0];
                var notif2 = notifications[1];

                var sentSuccess = new List<object>();
                var sentFailed = new List<object>();

                try
                {
                    await _notificationHelper.SendToUserAsync(notif1.UserId.ToString(), notif1);
                    sentSuccess.Add(new { UserId = notif1.UserId, NotificationId = notif1.NotificationId });
                }
                catch (Exception ex)
                {
                    sentFailed.Add(new { UserId = notif1.UserId, NotificationId = notif1.NotificationId, Error = ex.Message });
                }

                try
                {
                    await _notificationHelper.SendToUserAsync(notif2.UserId.ToString(), notif2);
                    sentSuccess.Add(new { UserId = notif2.UserId, NotificationId = notif2.NotificationId });
                }
                catch (Exception ex)
                {
                    sentFailed.Add(new { UserId = notif2.UserId, NotificationId = notif2.NotificationId, Error = ex.Message });
                }

                return Ok(new
                {
                    Message = "Attachment, Action rejected and Finding reopen successfully.",
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

    }
}
