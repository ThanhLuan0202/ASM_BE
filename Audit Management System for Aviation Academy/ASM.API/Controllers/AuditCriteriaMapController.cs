using ASM_Repositories.Models.AuditCriteriaMapDTO;
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
    public class AuditCriteriaMapController : ControllerBase
    {
        private readonly IAuditCriteriaMapService _service;

        public AuditCriteriaMapController(IAuditCriteriaMapService service)
        {
            _service = service;
        }

        [HttpGet("audit/{auditId}")]
        public async Task<ActionResult<IEnumerable<ViewAuditCriteriaMap>>> GetByAudit(Guid auditId)
        {
            try
            {
                if (auditId == Guid.Empty) return BadRequest(new { message = "Invalid audit ID" });
                var result = await _service.GetByAuditIdAsync(auditId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving mappings", error = ex.Message });
            }
        }

        [HttpGet("{auditId}/{criteriaId}")]
        public async Task<ActionResult<ViewAuditCriteriaMap>> Get(Guid auditId, Guid criteriaId)
        {
            try
            {
                if (auditId == Guid.Empty || criteriaId == Guid.Empty) return BadRequest(new { message = "Invalid IDs" });
                var result = await _service.GetAsync(auditId, criteriaId);
                if (result == null) return NotFound(new { message = "Mapping not found" });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the mapping", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewAuditCriteriaMap>> Create([FromBody] CreateAuditCriteriaMap dto)
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
                if (dto.AuditId == Guid.Empty || dto.CriteriaId == Guid.Empty) return BadRequest(new { message = "AuditId and CriteriaId are required" });

                var result = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(Get), new { auditId = result.AuditId, criteriaId = result.CriteriaId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the mapping", error = ex.Message });
            }
        }

        [HttpDelete("{auditId}/{criteriaId}")]
        public async Task<ActionResult> Delete(Guid auditId, Guid criteriaId)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (auditId == Guid.Empty || criteriaId == Guid.Empty) return BadRequest(new { message = "Invalid IDs" });
                var ok = await _service.DeleteAsync(auditId, criteriaId, userId);
                if (!ok) return NotFound(new { message = "Mapping not found" });
                return Ok(new { message = "Mapping deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the mapping", error = ex.Message });
            }
        }
    }
}
