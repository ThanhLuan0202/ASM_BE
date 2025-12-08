using ASM_Repositories.Models.AuditAssignmentDTO;
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditAssignmentController : ControllerBase
    {
        private readonly IAuditAssignmentService _service;
        private readonly IAuthService _authService;

        public AuditAssignmentController(IAuditAssignmentService service, IAuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewAuditAssignment>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit assignments", error = ex.Message });
            }
        }

        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<ViewAuditAssignment>> GetById(Guid assignmentId)
        {
            try
            {
                if (assignmentId == Guid.Empty)
                    return BadRequest(new { message = "Invalid AssignmentId" });

                var result = await _service.GetByIdAsync(assignmentId);
                if (result == null)
                    return NotFound(new { message = "Audit assignment not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the audit assignment", error = ex.Message });
            }
        }

        [HttpGet("audit/{auditId}")]
        public async Task<ActionResult<IEnumerable<ViewAuditAssignment>>> GetByAuditId(Guid auditId)
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
                return StatusCode(500, new { message = "An error occurred while retrieving audit assignments", error = ex.Message });
            }
        }

        [HttpGet("auditor/{auditorId}")]
        public async Task<ActionResult<IEnumerable<ViewAuditAssignment>>> GetByAuditorId(Guid auditorId)
        {
            try
            {
                if (auditorId == Guid.Empty)
                    return BadRequest(new { message = "Invalid AuditorId" });

                var result = await _service.GetByAuditorIdAsync(auditorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit assignments", error = ex.Message });
            }
        }

        [HttpGet("department/{deptId}")]
        public async Task<ActionResult<IEnumerable<ViewAuditAssignment>>> GetByDeptId(int deptId)
        {
            try
            {
                if (deptId <= 0)
                    return BadRequest(new { message = "Invalid DeptId" });

                var result = await _service.GetByDeptIdAsync(deptId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit assignments", error = ex.Message });
            }
        }

        [HttpGet("my-assignments")]
        public async Task<ActionResult<IEnumerable<ViewAuditAssignment>>> GetMyAssignments()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid auditorId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token" });

                var result = await _service.GetByAuditorIdAsync(auditorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit assignments", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewAuditAssignment>> Create([FromBody] CreateAuditAssignment dto)
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

                if (dto.AuditId == Guid.Empty)
                    return BadRequest(new { message = "AuditId is required" });

                if (dto.DeptId <= 0)
                    return BadRequest(new { message = "DeptId is required" });

                if (dto.AuditorId == Guid.Empty)
                    return BadRequest(new { message = "AuditorId is required" });

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { assignmentId = result.AssignmentId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the audit assignment", error = ex.Message });
            }
        }

        [HttpPut("{assignmentId}")]
        public async Task<ActionResult<ViewAuditAssignment>> Update(Guid assignmentId, [FromBody] UpdateAuditAssignment dto)
        {
            try
            {
                if (assignmentId == Guid.Empty)
                    return BadRequest(new { message = "Invalid AssignmentId" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var result = await _service.UpdateAsync(assignmentId, dto);
                if (result == null)
                    return NotFound(new { message = "Audit assignment not found" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the audit assignment", error = ex.Message });
            }
        }

        [HttpDelete("{assignmentId}")]
        public async Task<ActionResult> Delete(Guid assignmentId)
        {
            try
            {
                if (assignmentId == Guid.Empty)
                    return BadRequest(new { message = "Invalid AssignmentId" });

                var result = await _service.DeleteAsync(assignmentId);
                if (!result)
                    return NotFound(new { message = "Audit assignment not found" });

                return Ok(new { message = "Audit assignment deleted successfully (status changed to Inactive)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the audit assignment", error = ex.Message });
            }
        }

        [HttpGet("auditors-with-schedule")]
        public async Task<IActionResult> GetAuditorsWithSchedule()
        {
            try
            {
                var result = await _authService.GetAuditorsWithScheduleAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "An error occurred while retrieving auditors", 
                    error = ex.Message 
                });
            }
        }
    }
}

