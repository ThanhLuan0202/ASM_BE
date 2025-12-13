using ASM_Repositories.Models.DepartmentSensitiveAreaDTO;
using ASM_Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DepartmentSensitiveAreaController : ControllerBase
    {
        private readonly IDepartmentSensitiveAreaService _service;

        public DepartmentSensitiveAreaController(IDepartmentSensitiveAreaService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ViewDepartmentSensitiveArea>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving department sensitive areas", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ViewDepartmentSensitiveArea>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Id" });

                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = "Department sensitive area not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the department sensitive area", error = ex.Message });
            }
        }

        [HttpGet("department/{deptId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ViewDepartmentSensitiveArea>> GetByDeptId(int deptId)
        {
            try
            {
                if (deptId <= 0)
                    return BadRequest(new { message = "Invalid DeptId" });

                var result = await _service.GetByDeptIdAsync(deptId);
                if (result == null)
                    return NotFound(new { message = "Department sensitive area not found for this department" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the department sensitive area", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewDepartmentSensitiveArea>> Create([FromBody] CreateDepartmentSensitiveArea dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (dto.DeptId <= 0)
                    return BadRequest(new { message = "DeptId is required" });

                if (string.IsNullOrWhiteSpace(dto.SensitiveArea))
                    return BadRequest(new { message = "Sensitive area is required" });

                var result = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the department sensitive area", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ViewDepartmentSensitiveArea>> Update(Guid id, [FromBody] UpdateDepartmentSensitiveArea dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Id" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var result = await _service.UpdateAsync(id, dto, userId);
                if (result == null)
                    return NotFound(new { message = "Department sensitive area not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the department sensitive area", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (id == Guid.Empty)
                    return BadRequest(new { message = "Invalid Id" });

                var result = await _service.DeleteAsync(id, userId);
                if (!result)
                    return NotFound(new { message = "Department sensitive area not found" });

                return Ok(new { message = "Department sensitive area deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the department sensitive area", error = ex.Message });
            }
        }
    }
}

