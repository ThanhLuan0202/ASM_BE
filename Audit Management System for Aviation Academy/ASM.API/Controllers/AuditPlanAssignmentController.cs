using ASM.API.Helper;
using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditPlanAssignmentDTO;
using ASM_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditPlanAssignmentController : ControllerBase
    {
        private readonly IAuditPlanAssignmentService _service;
        private readonly ILogger<AuditPlanAssignmentController> _logger;
        private readonly NotificationHelper _notificationHelper;

        public AuditPlanAssignmentController(IAuditPlanAssignmentService service, ILogger<AuditPlanAssignmentController> logger, NotificationHelper notificationHelper)
        {
            _service = service;
            _logger = logger;
            _notificationHelper = notificationHelper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit plan assignments.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid AssignmentId" });

                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound("Audit plan assignment not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving audit plan assignment {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuditPlanAssignment dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("User not authenticated");

                if (!Guid.TryParse(userIdClaim, out Guid userIdGuid))
                {
                    return BadRequest(new { message = "Invalid UserId format in token." });
                }

                dto.AssignBy = userIdGuid;

                var (assignment, notif) = await _service.CreateWithNotificationAsync(dto, userIdGuid);

                if (notif != null)
                {
                    await _notificationHelper.SendToUserAsync(notif.UserId.ToString(), notif);
                }

                return Ok(new
                {
                    Message = $"AuditPlanAssignment created successfully.",
                    Assignment = assignment,
                    Notification = notif != null ? new
                    {
                        UserId = notif.UserId,
                        NotificationId = notif.NotificationId
                    } : null
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit plan assignment.");
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuditPlanAssignment dto)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid AssignmentId" });

                var existing = await _service.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { message = "Audit plan assignment not found." });

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                var result = await _service.UpdateAsync(id, dto, userId);
                if (result == null)
                    return NotFound(new { message = "Audit plan assignment not found." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating audit plan assignment {id}");
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid AssignmentId" });

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                var success = await _service.DeleteAsync(id, userId);
                if (!success)
                    return NotFound("Audit plan assignment not found or already inactive.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting audit plan assignment {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("by-period")]
        public async Task<IActionResult> GetAssignmentsByPeriod([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate >= endDate)
                {
                    return BadRequest(new { message = "StartDate must be earlier than EndDate" });
                }

                var result = await _service.GetAssignmentsByPeriodAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit plan assignments by period.");
                return StatusCode(500, new { message = "An error occurred while retrieving assignments by period", error = ex.Message });
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateAssignment([FromBody] ValidateAssignmentRequest request)
        {
            try
            {
                if (request.StartDate >= request.EndDate)
                {
                    return BadRequest(new { message = "StartDate must be earlier than EndDate" });
                }

                var result = await _service.ValidateAssignmentAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating audit plan assignment.");
                return StatusCode(500, new { message = "An error occurred while validating assignment", error = ex.Message });
            }
        }
    }
}
