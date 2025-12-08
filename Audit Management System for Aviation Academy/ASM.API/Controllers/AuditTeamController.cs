using ASM_Repositories.Models.AuditTeamDTO;
using ASM_Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditTeamController : ControllerBase
    {
        private readonly IAuditTeamService _service;

        public AuditTeamController(IAuditTeamService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            if (!result.Any())
                return NotFound(new { message = "No active audit teams found." });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = "Audit team not found." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuditTeam dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                var result = await _service.CreateAsync(dto, userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAuditTeam dto)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                var result = await _service.UpdateAsync(id, dto, userId);
                if (result == null) return NotFound(new { message = "Audit team not found." });
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(Guid id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                var success = await _service.SoftDeleteAsync(id, userId);
                if (!success)
                    return NotFound(new { message = "Record not found or already inactive." });
                return Ok(new { message = "Audit team member marked as inactive successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("check-lead-auditor/{auditId}")]
        [Authorize]
        public async Task<IActionResult> CheckIsLeadAuditor(Guid auditId)
        {
            try
            {
                if (auditId == Guid.Empty)
                    return BadRequest(new { message = "Invalid audit ID" });

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token" });

                var isLeadAuditor = await _service.IsLeadAuditorAsync(userId, auditId);

                return Ok(new { 
                    isLeadAuditor = isLeadAuditor,
                    userId = userId,
                    auditId = auditId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking lead auditor status", error = ex.Message });
            }
        }

        [HttpGet("my-lead-auditor-audits")]
        [Authorize]
        public async Task<IActionResult> GetMyLeadAuditorAudits()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token" });

                var auditIds = await _service.GetAuditIdsByLeadAuditorAsync(userId);

                return Ok(new { 
                    isLeadAuditor = auditIds.Any(),
                    userId = userId,
                    auditIds = auditIds,
                    count = auditIds.Count()
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving lead auditor audits", error = ex.Message });
            }
        }

        [HttpGet("auditors/{auditId}")]
        public async Task<IActionResult> GetAuditorsByAuditId(Guid auditId)
        {
            try
            {
                if (auditId == Guid.Empty)
                    return BadRequest(new { message = "Invalid audit ID" });

                var auditors = await _service.GetAuditorsByAuditIdAsync(auditId);

                if (!auditors.Any())
                    return NotFound(new { message = "No auditors found for this audit" });

                return Ok(auditors);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving auditors", error = ex.Message });
            }
        }

        [HttpGet("available-members")]
        public async Task<IActionResult> GetAvailableTeamMembers(
            [FromQuery] bool excludePreviousPeriod = false,
            [FromQuery] DateTime? previousPeriodStartDate = null,
            [FromQuery] DateTime? previousPeriodEndDate = null)
        {
            try
            {
                if (excludePreviousPeriod)
                {
                    if (!previousPeriodStartDate.HasValue || !previousPeriodEndDate.HasValue)
                    {
                        return BadRequest(new { message = "previousPeriodStartDate and previousPeriodEndDate are required when excludePreviousPeriod is true" });
                    }

                    if (previousPeriodStartDate.Value >= previousPeriodEndDate.Value)
                    {
                        return BadRequest(new { message = "previousPeriodStartDate must be earlier than previousPeriodEndDate" });
                    }
                }

                var result = await _service.GetAvailableTeamMembersAsync(
                    excludePreviousPeriod, 
                    previousPeriodStartDate, 
                    previousPeriodEndDate);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving available team members", error = ex.Message });
            }
        }
    }
}
