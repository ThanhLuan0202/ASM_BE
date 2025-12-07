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

        public AuditPlanAssignmentController(IAuditPlanAssignmentService service, ILogger<AuditPlanAssignmentController> logger)
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
                // Lấy UserId từ token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("User not authenticated");

                if (!Guid.TryParse(userIdClaim, out Guid userIdGuid))
                {
                    return BadRequest(new { message = "Invalid UserId format in token." });
                }

                dto.AssignBy = userIdGuid;

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.AssignmentId }, result);
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

                var result = await _service.UpdateAsync(id, dto);
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

                var success = await _service.DeleteAsync(id);
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
    }
}
