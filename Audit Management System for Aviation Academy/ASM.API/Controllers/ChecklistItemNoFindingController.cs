using ASM_Repositories.Models.ChecklistItemNoFindingDTO;
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
    public class ChecklistItemNoFindingController : ControllerBase
    {
        private readonly IChecklistItemNoFindingService _service;
        private readonly ILogger<ChecklistItemNoFindingController> _logger;

        public ChecklistItemNoFindingController(IChecklistItemNoFindingService service, ILogger<ChecklistItemNoFindingController> logger)
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
                _logger.LogError(ex, "Error retrieving checklist item no findings.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound("Checklist item no finding not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving checklist item no finding {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateChecklistItemNoFinding dto)
        {
            try
            {
                // Lấy UserId từ token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized("User not authenticated");

                // Set CreatedBy từ UserId trong token
                if (Guid.TryParse(userIdClaim, out Guid userIdGuid))
                {
                    // Convert Guid to int using hash code (ensure positive)
                    dto.CreatedBy = Math.Abs(userIdGuid.GetHashCode());
                }
                else if (int.TryParse(userIdClaim, out int userIdInt))
                {
                    dto.CreatedBy = userIdInt;
                }
                else
                {
                    return BadRequest(new { message = "Invalid UserId format in token." });
                }

                var result = await _service.CreateAsync(dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checklist item no finding.");
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChecklistItemNoFinding dto)
        {
            try
            {
                var existing = await _service.GetByIdAsync(id);
                if (existing == null)
                    return NotFound(new { message = "Checklist item no finding not found." });

                var result = await _service.UpdateAsync(id, dto);
                if (result == null)
                    return NotFound(new { message = "Checklist item no finding not found." });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating checklist item no finding {id}");
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success)
                    return NotFound("Checklist item no finding not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting checklist item no finding {id}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
