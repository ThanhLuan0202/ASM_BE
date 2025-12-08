using ASM_Repositories.Models.ActionStatusDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM.API.AdminControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminActionStatusController : ControllerBase
    {
        private readonly IActionStatusService _service;

        public AdminActionStatusController(IActionStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewActionStatus>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving action statuses", error = ex.Message });
            }
        }

        [HttpGet("{actionStatus}")]
        public async Task<ActionResult<ViewActionStatus>> GetById(string actionStatus)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(actionStatus))
                    return BadRequest(new { message = "ActionStatus is required" });

                var result = await _service.GetByIdAsync(actionStatus);
                if (result == null)
                    return NotFound(new { message = "ActionStatus not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the action status", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewActionStatus>> Create([FromBody] CreateActionStatus dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing user token" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (string.IsNullOrWhiteSpace(dto.ActionStatus1))
                    return BadRequest(new { message = "ActionStatus is required" });

                var result = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { actionStatus = result.ActionStatus1 }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the action status", error = ex.Message });
            }
        }

        [HttpPut("{actionStatus}")]
        public async Task<ActionResult<ViewActionStatus>> Update(string actionStatus, [FromBody] UpdateActionStatus dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing user token" });

                if (string.IsNullOrWhiteSpace(actionStatus))
                    return BadRequest(new { message = "ActionStatus is required" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (string.IsNullOrWhiteSpace(dto.ActionStatus1))
                    return BadRequest(new { message = "ActionStatus is required" });

                var result = await _service.UpdateAsync(actionStatus, dto, userId);
                if (result == null)
                    return NotFound(new { message = "ActionStatus not found" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the action status", error = ex.Message });
            }
        }

        [HttpDelete("{actionStatus}")]
        public async Task<ActionResult> Delete(string actionStatus)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing user token" });

                if (string.IsNullOrWhiteSpace(actionStatus))
                    return BadRequest(new { message = "ActionStatus is required" });

                var result = await _service.DeleteAsync(actionStatus, userId);
                if (!result)
                    return NotFound(new { message = "ActionStatus not found" });

                return Ok(new { message = "ActionStatus deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the action status", error = ex.Message });
            }
        }
    }
}

