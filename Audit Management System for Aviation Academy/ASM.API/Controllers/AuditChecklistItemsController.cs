using ASM_Repositories.Models.AuditChecklistItemDTO;
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
    public class AuditChecklistItemsController : ControllerBase
    {
        private readonly IAuditChecklistItemService _service;

        public AuditChecklistItemsController(IAuditChecklistItemService service)
        {
            _service = service;
        }

        [HttpGet("audit/{auditId}")]
        public async Task<ActionResult<IEnumerable<ViewAuditChecklistItem>>> GetByAuditId(Guid auditId)
        {
            try
            {
                if (auditId == Guid.Empty) return BadRequest(new { message = "Invalid audit ID" });
                var result = await _service.GetByAuditIdAsync(auditId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit checklist items", error = ex.Message });
            }
        }

        [HttpGet("{auditItemId}")]
        public async Task<ActionResult<ViewAuditChecklistItem>> GetById(Guid auditItemId)
        {
            try
            {
                if (auditItemId == Guid.Empty) return BadRequest(new { message = "Invalid item ID" });
                var result = await _service.GetByIdAsync(auditItemId);
                if (result == null) return NotFound(new { message = "Audit checklist item not found" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the item", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewAuditChecklistItem>> Create([FromBody] CreateAuditChecklistItem dto)
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
                if (dto.AuditId == Guid.Empty) return BadRequest(new { message = "AuditId is required" });
                if (string.IsNullOrWhiteSpace(dto.QuestionTextSnapshot)) return BadRequest(new { message = "QuestionTextSnapshot is required" });

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { auditItemId = result.AuditItemId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the item", error = ex.Message });
            }
        }

        [HttpPut("{auditItemId}")]
        public async Task<ActionResult<ViewAuditChecklistItem>> Update(Guid auditItemId, [FromBody] UpdateAuditChecklistItem dto)
        {
            try
            {
                if (auditItemId == Guid.Empty) return BadRequest(new { message = "Invalid item ID" });
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }
                var result = await _service.UpdateAsync(auditItemId, dto);
                if (result == null) return NotFound(new { message = "Audit checklist item not found" });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the item", error = ex.Message });
            }
        }

        [HttpDelete("{auditItemId}")]
        public async Task<ActionResult> Delete(Guid auditItemId)
        {
            try
            {
                if (auditItemId == Guid.Empty) return BadRequest(new { message = "Invalid item ID" });
                var ok = await _service.DeleteAsync(auditItemId);
                if (!ok) return NotFound(new { message = "Audit checklist item not found" });
                return Ok(new { message = "Audit checklist item deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the item", error = ex.Message });
            }
        }

        [HttpGet("by-department/{deptId:int}")]
        public async Task<ActionResult<IEnumerable<ViewAuditChecklistItem>>> GetByDepartment(int deptId)
        {
            try
            {
                if (deptId <= 0)
                    return BadRequest(new { message = "DepartmentId must be greater than zero" });

                var result = await _service.GetBySectionAsync(deptId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit checklist items by section", error = ex.Message });
            }
        }

        [HttpGet("my-assignments")]
        public async Task<ActionResult<IEnumerable<ViewAuditChecklistItem>>> GetMyAssignments()
        {
            try
            {
                // Lấy UserId từ JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                var result = await _service.GetByUserIdAsync(userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit checklist items for user", error = ex.Message });
            }
        }
    }
}
