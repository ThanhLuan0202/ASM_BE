using ASM_Repositories.Models.AuditScheduleDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditScheduleController : ControllerBase
    {
        private readonly IAuditScheduleService _service;

        public AuditScheduleController(IAuditScheduleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewAuditSchedule>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit schedules", error = ex.Message });
            }
        }

        [HttpGet("{scheduleId}")]
        public async Task<ActionResult<ViewAuditSchedule>> GetById(Guid scheduleId)
        {
            try
            {
                if (scheduleId == Guid.Empty)
                    return BadRequest(new { message = "Invalid ScheduleId" });

                var result = await _service.GetByIdAsync(scheduleId);
                if (result == null)
                    return NotFound(new { message = "Audit schedule not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the audit schedule", error = ex.Message });
            }
        }

        [HttpGet("audit/{auditId}")]
        public async Task<ActionResult<IEnumerable<ViewAuditSchedule>>> GetByAuditId(Guid auditId)
        {
            try
            {
                if (auditId == Guid.Empty)
                    return BadRequest(new { message = "Invalid AuditId" });

                var result = await _service.GetByAuditIdAsync(auditId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit schedules", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewAuditSchedule>> Create([FromBody] CreateAuditSchedule dto)
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

                if (dto.AuditId == Guid.Empty)
                    return BadRequest(new { message = "AuditId is required" });

                if (string.IsNullOrWhiteSpace(dto.MilestoneName))
                    return BadRequest(new { message = "MilestoneName is required" });

                var result = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { scheduleId = result.ScheduleId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the audit schedule", error = ex.Message });
            }
        }

        [HttpPut("{scheduleId}")]
        public async Task<ActionResult<ViewAuditSchedule>> Update(Guid scheduleId, [FromBody] UpdateAuditSchedule dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (scheduleId == Guid.Empty)
                    return BadRequest(new { message = "Invalid ScheduleId" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (string.IsNullOrWhiteSpace(dto.MilestoneName))
                    return BadRequest(new { message = "MilestoneName is required" });

                var result = await _service.UpdateAsync(scheduleId, dto, userId);
                if (result == null)
                    return NotFound(new { message = "Audit schedule not found" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the audit schedule", error = ex.Message });
            }
        }

        [HttpDelete("{scheduleId}")]
        public async Task<ActionResult> Delete(Guid scheduleId)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (scheduleId == Guid.Empty)
                    return BadRequest(new { message = "Invalid ScheduleId" });

                var result = await _service.DeleteAsync(scheduleId, userId);
                if (!result)
                    return NotFound(new { message = "Audit schedule not found" });

                return Ok(new { message = "Audit schedule deleted successfully (status changed to Inactive)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the audit schedule", error = ex.Message });
            }
        }
    }
}

