using ASM_Repositories.Models.RoleDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{roleName}")]
        public async Task<IActionResult> GetById(string roleName)
        {
            var result = await _service.GetByIdAsync(roleName);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRole dto)
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
        }

        [HttpPut("{roleName}")]
        public async Task<IActionResult> Update(string roleName, [FromBody] UpdateRole dto)
        {
            var result = await _service.UpdateAsync(roleName, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{roleName}")]
        public async Task<IActionResult> Delete(string roleName)
        {
            var success = await _service.DeleteAsync(roleName);
            if (!success) return NotFound();
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
