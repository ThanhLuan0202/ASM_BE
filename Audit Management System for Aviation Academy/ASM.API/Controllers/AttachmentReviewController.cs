using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentReviewController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        public AttachmentReviewController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost("{attachmentId:guid}/approve")]
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
        public async Task<IActionResult> Reject(Guid attachmentId)
        {
            try
            {
                await _attachmentService.UpdateAttachmentStatusAsync(attachmentId, "Returned");
                return Ok(new { message = $"Attachment {attachmentId} returned successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
