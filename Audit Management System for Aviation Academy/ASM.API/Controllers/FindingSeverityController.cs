using ASM_Repositories.Models.FindingSeverityDTO;
using ASM_Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FindingSeverityController : ControllerBase
    {
        private readonly IFindingSeverityService _service;

        public FindingSeverityController(IFindingSeverityService service)
        {
            _service = service;
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
                return StatusCode(500, new
                {
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet("{severity}")]
        public async Task<IActionResult> GetById(string severity)
        {
            try
            {
                var result = await _service.GetByIdAsync(severity);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFindingSeverity dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var result = await _service.CreateAsync(dto, userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPut("{severity}")]
        public async Task<IActionResult> Update(string severity, [FromBody] UpdateFindingSeverity dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                var result = await _service.UpdateAsync(severity, dto, userId);
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

        [HttpDelete("{severity}")]
        public async Task<IActionResult> Delete(string severity)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                await _service.DeleteAsync(severity, userId);
                return Ok(new { message = "Deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

    }
}
