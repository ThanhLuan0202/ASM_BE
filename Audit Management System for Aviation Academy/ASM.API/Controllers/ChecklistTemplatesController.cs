using ASM_Repositories.Models.ChecklistTemplateDTO;
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
    public class ChecklistTemplatesController : ControllerBase
    {
        private readonly IChecklistTemplateService _service;

        public ChecklistTemplatesController(IChecklistTemplateService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewChecklistTemplate>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllChecklistTemplateAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving checklist templates", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewChecklistTemplate>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid template ID" });
                }

                var result = await _service.GetChecklistTemplateByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Checklist template with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the checklist template", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewChecklistTemplate>> Create([FromBody] CreateChecklistTemplate dto)
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

                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    return BadRequest(new { message = "Name is required" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                Guid? userId = null;
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedUserId))
                {
                    userId = parsedUserId;
                }

                var result = await _service.CreateChecklistTemplateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.TemplateId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the checklist template", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ViewChecklistTemplate>> Update(Guid id, [FromBody] UpdateChecklistTemplate dto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid template ID" });
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

                var result = await _service.UpdateChecklistTemplateAsync(id, dto);
                if (result == null)
                {
                    return NotFound(new { message = $"Checklist template with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the checklist template", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid template ID" });
                }

                var result = await _service.DeleteChecklistTemplateAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Checklist template with ID {id} not found" });
                }

                return Ok(new { message = "Checklist template deleted successfully (IsActive set to false)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the checklist template", error = ex.Message });
            }
        }
    }
}

