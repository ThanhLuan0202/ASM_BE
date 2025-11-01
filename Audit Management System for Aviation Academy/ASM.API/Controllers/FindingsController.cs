using ASM_Repositories.Models.FindingDTO;
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
    [Route("api/[controller]")]
    [ApiController]
    public class FindingsController : ControllerBase
    {
        private readonly IFindingService _service;

        public FindingsController(IFindingService service)
        {
            _service = service;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewFinding>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllFindingAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving findings", error = ex.Message });
            }
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<ViewFinding>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid finding ID" });
                }

                var result = await _service.GetFindingByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Finding with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the finding", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewFinding>> Create([FromBody] CreateFinding dto)
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

                if (dto.AuditId == Guid.Empty)
                {
                    return BadRequest(new { message = "AuditId is required" });
                }

                // Get UserId from JWT token claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                Guid? userId = null;
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var result = await _service.CreateFindingAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.FindingId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the finding", error = ex.Message });
            }
        }

    
        [HttpPut("{id}")]
        public async Task<ActionResult<ViewFinding>> Update(Guid id, [FromBody] UpdateFinding dto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid finding ID" });
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

                var result = await _service.UpdateFindingAsync(id, dto);
                if (result == null)
                {
                    return NotFound(new { message = $"Finding with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the finding", error = ex.Message });
            }
        }

        
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid finding ID" });
                }

                var result = await _service.DeleteFindingAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Finding with ID {id} not found" });
                }

                return Ok(new { message = "Finding deleted successfully (status changed to Inactive)" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the finding", error = ex.Message });
            }
        }
    }
}
