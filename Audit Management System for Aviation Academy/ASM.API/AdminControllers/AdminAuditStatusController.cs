using ASM_Repositories.Models.AuditStatusDTO;
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
    public class AdminAuditStatusController : ControllerBase
    {
        private readonly IAuditStatusService _service;

        public AdminAuditStatusController(IAuditStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewAuditStatus>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving audit statuses", error = ex.Message });
            }
        }

        [HttpGet("{auditStatus}")]
        public async Task<ActionResult<ViewAuditStatus>> GetById(string auditStatus)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(auditStatus))
                    return BadRequest(new { message = "AuditStatus is required" });

                var result = await _service.GetByIdAsync(auditStatus);
                if (result == null)
                    return NotFound(new { message = "AuditStatus not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the audit status", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewAuditStatus>> Create([FromBody] CreateAuditStatus dto)
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

                if (string.IsNullOrWhiteSpace(dto.AuditStatus1))
                    return BadRequest(new { message = "AuditStatus is required" });

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { auditStatus = result.AuditStatus1 }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the audit status", error = ex.Message });
            }
        }

        [HttpPut("{auditStatus}")]
        public async Task<ActionResult<ViewAuditStatus>> Update(string auditStatus, [FromBody] UpdateAuditStatus dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(auditStatus))
                    return BadRequest(new { message = "AuditStatus is required" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (string.IsNullOrWhiteSpace(dto.AuditStatus1))
                    return BadRequest(new { message = "AuditStatus is required" });

                var result = await _service.UpdateAsync(auditStatus, dto);
                if (result == null)
                    return NotFound(new { message = "AuditStatus not found" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the audit status", error = ex.Message });
            }
        }

        [HttpDelete("{auditStatus}")]
        public async Task<ActionResult> Delete(string auditStatus)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(auditStatus))
                    return BadRequest(new { message = "AuditStatus is required" });

                var result = await _service.DeleteAsync(auditStatus);
                if (!result)
                    return NotFound(new { message = "AuditStatus not found" });

                return Ok(new { message = "AuditStatus deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the audit status", error = ex.Message });
            }
        }
    }
}

