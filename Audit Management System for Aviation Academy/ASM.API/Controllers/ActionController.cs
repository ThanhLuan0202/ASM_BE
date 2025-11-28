using ASM_Repositories.Models.ActionDTO;
using ASM_Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class ActionController : ControllerBase
{
    private readonly IActionService _service;
    private readonly ILogger<ActionController> _logger;

    public class UpdateProgressPercentRequest
    {
        [Range(0, 100, ErrorMessage = "ProgressPercent must be between 0 and 100.")]
        public byte ProgressPercent { get; set; }
    }

    public ActionController(IActionService service, ILogger<ActionController> logger)
    {
        _service = service;
        _logger = logger;
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
            _logger.LogError(ex, "Error retrieving actions.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound("Action not found.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving action {id}");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("my-assigned")]
    public async Task<IActionResult> GetMyAssignedActions()
    {
        try
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized(new { message = "Invalid or missing UserId in token." });

            var result = await _service.GetByAssignedToAsync(userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving actions assigned to current user.");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAction dto)
    {
        try
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized(new { message = "Invalid or missing UserId in token." });

            dto.AssignedBy = userId;

            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
        }
    }




    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAction dto)
    {
        try
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized(new { message = "Invalid or missing UserId in token." });

            var existing = await _service.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Action not found." });

            if (existing.AssignedBy != userId)
                return Forbid("You are not authorized to update this action.");

            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound("Action not found or already inactive.");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting action {id}");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("{id}/status/in-progress")]
    public async Task<IActionResult> SetStatusInProgress(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ActionId" });

            var updated = await _service.UpdateStatusToInProgressAsync(id);
            if (!updated)
                return NotFound(new { message = "Action not found or inactive." });

            return Ok(new { message = "Action status updated to InProgress." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating status of action {id} to InProgress");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("{id}/status/reviewed")]
    public async Task<IActionResult> SetStatusReviewed(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ActionId" });

            var updated = await _service.UpdateStatusToReviewedAsync(id);
            if (!updated)
                return NotFound(new { message = "Action not found or inactive." });

            return Ok(new { message = "Action status updated to Reviewed." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating status of action {id} to Reviewed");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("{id}/status/approved")]
    public async Task<IActionResult> SetStatusApproved(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ActionId" });

            var updated = await _service.UpdateStatusToApprovedAsync(id);
            if (!updated)
                return NotFound(new { message = "Action not found or inactive." });

            return Ok(new { message = "Action status updated to Approved." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating status of action {id} to Approved");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("{id}/status/rejected")]
    public async Task<IActionResult> SetStatusRejected(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ActionId" });

            var updated = await _service.UpdateStatusToRejectedAsync(id);
            if (!updated)
                return NotFound(new { message = "Action not found or inactive." });

            return Ok(new { message = "Action status updated to Rejected." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating status of action {id} to Rejected");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPut("{id}/progress-percent")]
    public async Task<IActionResult> UpdateProgressPercent(Guid id, [FromBody] UpdateProgressPercentRequest request)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ActionId" });

            if (request == null)
                return BadRequest(new { message = "Request body is required." });

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { message = "Validation failed", errors });
            }

            var updated = await _service.UpdateProgressPercentAsync(id, request.ProgressPercent);
            if (!updated)
                return NotFound(new { message = "Action not found or inactive." });

            return Ok(new { message = "Progress percent updated successfully.", progressPercent = request.ProgressPercent });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating progress percent for action {id}");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPost("{id}/status/closed")]
    public async Task<IActionResult> SetStatusClosed(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ActionId" });

            var updated = await _service.UpdateStatusToClosedAsync(id);
            if (!updated)
                return NotFound(new { message = "Action not found or inactive." });

            return Ok(new { message = "Action status updated to Closed." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating status of action {id} to Closed");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpPut("{id}/approve-by-auditor")]
    public async Task<IActionResult> ApproveByAuditor(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                return BadRequest(new { message = "Invalid ActionId" });

            var updated = await _service.UpdateStatusToApprovedAuditorAsync(id);
            if (!updated)
                return NotFound(new { message = "Action not found or inactive." });

            return Ok(new { message = "Action status updated to ApprovedAuditor." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating status of action {id} to ApprovedAuditor");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("by-finding/{findingId}")]
    public async Task<IActionResult> GetByFindingId(Guid findingId)
    {
        try
        {
            if (findingId == Guid.Empty)
                return BadRequest(new { message = "Invalid FindingId" });

            var result = await _service.GetByFindingIdAsync(findingId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving actions by findingId {findingId}");
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpGet("by-assigned-dept/{assignedDeptId:int}")]
    public async Task<IActionResult> GetByAssignedDeptId(int assignedDeptId)
    {
        try
        {
            if (assignedDeptId <= 0)
                return BadRequest(new { message = "Invalid AssignedDeptId" });

            var result = await _service.GetByAssignedDeptIdAsync(assignedDeptId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving actions by assignedDeptId {assignedDeptId}");
            return StatusCode(500, "Internal server error.");
        }
    }
}
