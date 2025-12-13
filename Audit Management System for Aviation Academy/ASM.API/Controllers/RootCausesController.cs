using ASM_Repositories.Models.RootCauseDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RootCausesController : ControllerBase
    {
        private readonly IRootCauseService _service;

        public RootCausesController(IRootCauseService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewRootCause>>> GetAll([FromQuery] string status = null, [FromQuery] string category = null)
        {
            try
            {
                IEnumerable<ViewRootCause> result;

                if (!string.IsNullOrWhiteSpace(status))
                {
                    result = await _service.GetByStatusAsync(status);
                }
                else if (!string.IsNullOrWhiteSpace(category))
                {
                    result = await _service.GetByCategoryAsync(category);
                }
                else
                {
                    result = await _service.GetAllAsync();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving root causes", error = ex.Message });
            }
        }

        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<ViewRootCause>>> GetByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    return BadRequest(new { message = "Status is required" });
                }

                var result = await _service.GetByStatusAsync(status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving root causes by status", error = ex.Message });
            }
        }

        [HttpGet("by-category/{category}")]
        public async Task<ActionResult<IEnumerable<ViewRootCause>>> GetByCategory(string category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                {
                    return BadRequest(new { message = "Category is required" });
                }

                var result = await _service.GetByCategoryAsync(category);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving root causes by category", error = ex.Message });
            }
        }

        [HttpGet("by-department/{deptId}")]
        public async Task<ActionResult<IEnumerable<ViewRootCause>>> GetByDeptId(int deptId)
        {
            try
            {
                if (deptId <= 0)
                {
                    return BadRequest(new { message = "Invalid department ID" });
                }

                var result = await _service.GetByDeptIdAsync(deptId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving root causes by department", error = ex.Message });
            }
        }

        [HttpGet("by-finding/{findingId}")]
        public async Task<ActionResult<IEnumerable<ViewRootCause>>> GetByFindingId(Guid findingId)
        {
            try
            {
                if (findingId == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid finding ID" });
                }

                var result = await _service.GetByFindingIdAsync(findingId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving root causes by finding", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewRootCause>> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Invalid root cause ID" });
                }

                var result = await _service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Root cause with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the root cause", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewRootCause>> Create([FromBody] CreateRootCause dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new
                        {
                            Field = x.Key,
                            Message = e.ErrorMessage
                        }))
                        .ToList();

                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = errors
                    });
                }

                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest(new { message = "Name is required" });
                }

                // Set default status if not provided
                if (string.IsNullOrWhiteSpace(dto.Status))
                {
                    dto.Status = "Active";
                }

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.RootCauseId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the root cause", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ViewRootCause>> Update(int id, [FromBody] UpdateRootCause dto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Invalid root cause ID" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new
                        {
                            Field = x.Key,
                            Message = e.ErrorMessage
                        }))
                        .ToList();

                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = errors
                    });
                }

                var result = await _service.UpdateAsync(id, dto);
                if (result == null)
                {
                    return NotFound(new { message = $"Root cause with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the root cause", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "Invalid root cause ID" });
                }

                var result = await _service.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Root cause with ID {id} not found" });
                }

                return Ok(new { message = "Root cause deleted successfully (status changed to Inactive)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the root cause", error = ex.Message });
            }
        }
    }
}
