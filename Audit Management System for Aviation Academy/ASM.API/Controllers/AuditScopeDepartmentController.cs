using ASM_Repositories.Models.AuditScopeDepartmentDTO;
using ASM_Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditScopeDepartmentController : ControllerBase
    {
        private readonly IAuditScopeDepartmentService _service;

        public AuditScopeDepartmentController(IAuditScopeDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.Any()) return NotFound(new { message = "No active audit-scope-department records found." });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "AuditScopeDepartment not found." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuditScopeDepartment dto)
        {
            try
            {
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuditScopeDepartment dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);
                if (result == null) return NotFound(new { message = "AuditScopeDepartment not found." });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            var success = await _service.SoftDeleteAsync(id);
            if (!success) return NotFound(new { message = "Record not found or already inactive." });
            return Ok(new { message = "Record marked as inactive successfully." });
        }
    }
}
