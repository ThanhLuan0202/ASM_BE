using ASM_Repositories.Models.AuditDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditsController : ControllerBase
    {
        private readonly IAuditService _service;

        public AuditsController(IAuditService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewAudit>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAuditAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audits", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewAudit>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                var result = await _service.GetAuditByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the audit", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewAudit>> Create([FromBody] CreateAudit dto)
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

                if (string.IsNullOrWhiteSpace(dto.Title))
                {
                    return BadRequest(new { message = "Title is required" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                Guid? userId = null;
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var result = await _service.CreateAuditAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.AuditId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the audit", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ViewAudit>> Update(Guid id, [FromBody] UpdateAudit dto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
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

                var result = await _service.UpdateAuditAsync(id, dto);
                if (result == null)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the audit", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                var result = await _service.DeleteAuditAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(new { message = "Audit deleted successfully (status changed to Inactive)" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the audit", error = ex.Message });
            }
        }

        public class SubmitToLeadAuditorRequest
        {
            public string Comment { get; set; }
        }

        [HttpPost("{id}/submit-to-lead-auditor")]
        public async Task<ActionResult> SubmitToLeadAuditor(Guid id, [FromBody] SubmitToLeadAuditorRequest request)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid audit ID" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid approverId))
                {
                    return Unauthorized(new { message = "Invalid or missing user token" });
                }

                var ok = await _service.SubmitToLeadAuditorAsync(id, approverId, request?.Comment);
                if (!ok)
                {
                    return NotFound(new { message = $"Audit with ID {id} not found" });
                }

                return Ok(new { message = "Submitted to Lead Auditor successfully. Audit status set to PendingReview." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while submitting the audit", error = ex.Message });
            }
        }
    }
}

