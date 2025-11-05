using ASM_Repositories.Models.FindingStatusDTO;
using ASM_Services.Interfaces.DepartmentHeadInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FindingStatusController : ControllerBase
    {
        private readonly IFindingStatusService _service;

        public FindingStatusController(IFindingStatusService service)
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
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("{status}")]
        public async Task<IActionResult> GetById(string status)
        {
            try
            {
                var result = await _service.GetByIdAsync(status);
                if (result == null)
                    return NotFound(new { message = $"FindingStatus '{status}' not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFindingStatus dto)
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

        [HttpPut("{status}")]
        public async Task<IActionResult> Update(string status, [FromBody] UpdateFindingStatus dto)
        {
            try
            {
                var exists = await _service.GetByIdAsync(status);
                if (exists == null)
                    return NotFound(new { message = $"FindingStatus '{status}' not found." });

                var success = await _service.UpdateAsync(status, dto);
                if (!success)
                    return BadRequest(new { message = "Update failed." });

                return Ok(new { message = "Updated successfully." });
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

        [HttpDelete("{status}")]
        public async Task<IActionResult> Delete(string status)
        {
            try
            {
                var exists = await _service.GetByIdAsync(status);
                if (exists == null)
                    return NotFound(new { message = $"FindingStatus '{status}' not found." });

                var success = await _service.DeleteAsync(status);
                if (!success)
                    return BadRequest(new { message = "Delete failed." });

                return Ok(new { message = "Deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}