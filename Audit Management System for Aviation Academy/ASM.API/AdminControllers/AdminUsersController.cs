using ASM_Repositories.Models.UsersDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.AdminInterfaces.AdminServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM.API.AdminControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUsersService _service;

        public AdminUsersController(IUsersService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _service.GetAllAsync();
                if (!users.Any())
                    return NotFound("No users found.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var user = await _service.GetByIdAsync(id);
                if (user == null)
                    return NotFound("User not found.");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                var created = await _service.CreateAsync(model, userId);
                return CreatedAtAction(nameof(GetById), new { id = created.UserId }, created);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { error = "Internal server error", detail = message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUser model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                var updated = await _service.UpdateAsync(id, model, userId);
                if (updated == null) return NotFound("User not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { error = "Internal server error", detail = message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                var result = await _service.DeleteAsync(id, userId);
                if (!result)
                    return NotFound("User not found or already inactive.");
                return Ok("User has been set to inactive.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-department/{deptId}")]
        public async Task<IActionResult> GetByDeptId(int deptId)
        {
            try
            {
                if (deptId <= 0)
                    return BadRequest(new { message = "DeptId must be greater than zero" });

                var users = await _service.GetByDeptIdAsync(deptId);
                if (!users.Any())
                    return NotFound("No users found for this department.");
                return Ok(users);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

