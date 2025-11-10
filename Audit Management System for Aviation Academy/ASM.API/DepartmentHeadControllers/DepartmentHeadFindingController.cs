using ASM_Repositories.Interfaces.DepartmentHeadInterfaces;
using ASM_Services.Services.SQAStaffServices;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM.API.DepartmentHeadControllers
{
    [Route("api/department-head/[controller]")]
    [ApiController]
    public class DepartmentHeadFindingController : ControllerBase
    {
        private readonly IDepartmentHeadFindingRepository _service;

        public DepartmentHeadFindingController(IDepartmentHeadFindingRepository service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetFindingsByMyDepartment()
        {
            var deptIdClaim = User.Claims.FirstOrDefault(c => c.Type == "DeptId")?.Value;
            if (string.IsNullOrEmpty(deptIdClaim) || !int.TryParse(deptIdClaim, out int deptId))
                return BadRequest("Department ID not found in token.");

            var result = await _service.GetFindingsByDepartmentAsync(deptId);

            if (result == null || !result.Any())
                return Ok(new { message = "No tasks found." });

            return Ok(result);
        }
        [HttpGet("{deptId}")]
        public async Task<IActionResult> GetFindingsByMyDepartment(int deptId)
        {
            try
            {
                var result = await _service.GetFindingsByDepartmentAsync(deptId);

                if (result == null || !result.Any())
                {
                    return NotFound(new { message = "No tasks found for DepartmentId: " + deptId });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


    }
}
