using ASM_Repositories.Models.DepartmentHeadDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.API.AdminControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminDepartmentHeadController : ControllerBase
    {
        private readonly IDepartmentHeadService _service;

        public AdminDepartmentHeadController(IDepartmentHeadService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewDepartmentHead>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving department heads", error = ex.Message });
            }
        }

        [HttpGet("{deptHeadId}")]
        public async Task<ActionResult<ViewDepartmentHead>> GetById(Guid deptHeadId)
        {
            try
            {
                if (deptHeadId == Guid.Empty)
                    return BadRequest(new { message = "Invalid DeptHeadId" });

                var result = await _service.GetByIdAsync(deptHeadId);
                if (result == null)
                    return NotFound(new { message = "DepartmentHead not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the department head", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewDepartmentHead>> Create([FromBody] CreateDepartmentHead dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (dto.DeptId <= 0)
                    return BadRequest(new { message = "DeptId is required and must be greater than 0" });

                if (dto.UserId == Guid.Empty)
                    return BadRequest(new { message = "UserId is required" });

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { deptHeadId = result.DeptHeadId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the department head", error = ex.Message });
            }
        }

        [HttpPut("{deptHeadId}")]
        public async Task<ActionResult<ViewDepartmentHead>> Update(Guid deptHeadId, [FromBody] UpdateDepartmentHead dto)
        {
            try
            {
                if (deptHeadId == Guid.Empty)
                    return BadRequest(new { message = "Invalid DeptHeadId" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (dto.DeptId <= 0)
                    return BadRequest(new { message = "DeptId is required and must be greater than 0" });

                if (dto.UserId == Guid.Empty)
                    return BadRequest(new { message = "UserId is required" });

                var result = await _service.UpdateAsync(deptHeadId, dto);
                if (result == null)
                    return NotFound(new { message = "DepartmentHead not found" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the department head", error = ex.Message });
            }
        }

        [HttpDelete("{deptHeadId}")]
        public async Task<ActionResult> Delete(Guid deptHeadId)
        {
            try
            {
                if (deptHeadId == Guid.Empty)
                    return BadRequest(new { message = "Invalid DeptHeadId" });

                var result = await _service.DeleteAsync(deptHeadId);
                if (!result)
                    return NotFound(new { message = "DepartmentHead not found" });

                return Ok(new { message = "DepartmentHead deleted successfully (status changed to Inactive)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the department head", error = ex.Message });
            }
        }
    }
}

