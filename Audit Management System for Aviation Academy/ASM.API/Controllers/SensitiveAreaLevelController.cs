using ASM_Repositories.Models.SensitiveAreaLevelDTO;
using ASM_Services.Interfaces;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SensitiveAreaLevelController : ControllerBase
    {
        private readonly ISensitiveAreaLevelService _service;

        public SensitiveAreaLevelController(ISensitiveAreaLevelService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
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

        [HttpGet("{level}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string level)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(level))
                    return BadRequest(new { message = "Level is required" });

                var result = await _service.GetByIdAsync(level);
                if (result == null)
                    return NotFound(new { message = "Level not found" });

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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSensitiveAreaLevel dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var result = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { level = result.Level }, result);
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

        [HttpPut("{level}")]
        public async Task<IActionResult> Update(string level, [FromBody] UpdateSensitiveAreaLevel dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                if (string.IsNullOrWhiteSpace(level))
                    return BadRequest(new { message = "Level is required" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var result = await _service.UpdateAsync(level, dto, userId);
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

        [HttpDelete("{level}")]
        public async Task<IActionResult> Delete(string level)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                if (string.IsNullOrWhiteSpace(level))
                    return BadRequest(new { message = "Level is required" });

                var success = await _service.DeleteAsync(level, userId);
                if (!success)
                    return NotFound(new { message = "Level not found" });

                return Ok(new { message = "Deleted successfully." });
            }
            catch (InvalidOperationException ex)
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

