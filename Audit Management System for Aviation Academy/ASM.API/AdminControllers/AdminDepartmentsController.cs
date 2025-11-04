using ASM_Repositories.Models;
using ASM_Repositories.Models.DepartmentDTO;
using ASM_Services.Interfaces.AdminInterfaces.AdminServices;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM.API.AdminControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminDepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public AdminDepartmentsController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllDepartmentsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetDepartmentByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDepartment dto)
        {
            await _service.CreateDepartmentAsync(dto);
            return Ok(new { message = "Created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartment dto)
        {
            await _service.UpdateDepartmentAsync(id, dto);
            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteDepartmentAsync(id);
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
