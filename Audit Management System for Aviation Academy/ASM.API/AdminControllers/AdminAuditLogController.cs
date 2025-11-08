using ASM_Repositories.Models.AuditLogDTO;
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
    public class AdminAuditLogController : ControllerBase
    {
        private readonly IAuditLogService _service;

        public AdminAuditLogController(IAuditLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewAuditLog>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit logs", error = ex.Message });
            }
        }

        [HttpGet("{logId}")]
        public async Task<ActionResult<ViewAuditLog>> GetById(Guid logId)
        {
            try
            {
                if (logId == Guid.Empty)
                    return BadRequest(new { message = "Invalid LogId" });

                var result = await _service.GetByIdAsync(logId);
                if (result == null)
                    return NotFound(new { message = "AuditLog not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the audit log", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewAuditLog>> Create([FromBody] CreateAuditLog dto)
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

                if (string.IsNullOrWhiteSpace(dto.EntityType))
                    return BadRequest(new { message = "EntityType is required" });

                if (string.IsNullOrWhiteSpace(dto.Action))
                    return BadRequest(new { message = "Action is required" });

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { logId = result.LogId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the audit log", error = ex.Message });
            }
        }

        [HttpPut("{logId}")]
        public async Task<ActionResult<ViewAuditLog>> Update(Guid logId, [FromBody] UpdateAuditLog dto)
        {
            try
            {
                if (logId == Guid.Empty)
                    return BadRequest(new { message = "Invalid LogId" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (string.IsNullOrWhiteSpace(dto.EntityType))
                    return BadRequest(new { message = "EntityType is required" });

                if (string.IsNullOrWhiteSpace(dto.Action))
                    return BadRequest(new { message = "Action is required" });

                var result = await _service.UpdateAsync(logId, dto);
                if (result == null)
                    return NotFound(new { message = "AuditLog not found" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the audit log", error = ex.Message });
            }
        }

        [HttpDelete("{logId}")]
        public async Task<ActionResult> Delete(Guid logId)
        {
            try
            {
                if (logId == Guid.Empty)
                    return BadRequest(new { message = "Invalid LogId" });

                var result = await _service.DeleteAsync(logId);
                if (!result)
                    return NotFound(new { message = "AuditLog not found" });

                return Ok(new { message = "AuditLog deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the audit log", error = ex.Message });
            }
        }
    }
}

