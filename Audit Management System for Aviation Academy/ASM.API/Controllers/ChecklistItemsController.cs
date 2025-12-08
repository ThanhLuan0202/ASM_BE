using ASM_Repositories.Models.ChecklistItemDTO;
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
    public class ChecklistItemsController : ControllerBase
    {
        private readonly IChecklistItemService _service;

        public ChecklistItemsController(IChecklistItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewChecklistItem>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllChecklistItemAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving checklist items", error = ex.Message });
            }
        }

        [HttpGet("template/{templateId}")]
        public async Task<ActionResult<IEnumerable<ViewChecklistItem>>> GetByTemplateId(Guid templateId)
        {
            try
            {
                if (templateId == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid template ID" });
                }

                var result = await _service.GetChecklistItemsByTemplateIdAsync(templateId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving checklist items", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ViewChecklistItem>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid item ID" });
                }

                var result = await _service.GetChecklistItemByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = $"Checklist item with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the checklist item", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewChecklistItem>> Create([FromBody] CreateChecklistItem dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
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

                if (string.IsNullOrWhiteSpace(dto.QuestionText))
                {
                    return BadRequest(new { message = "QuestionText is required" });
                }

                if (dto.TemplateId == Guid.Empty)
                {
                    return BadRequest(new { message = "TemplateId is required" });
                }

                var result = await _service.CreateChecklistItemAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.ItemId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the checklist item", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ViewChecklistItem>> Update(Guid id, [FromBody] UpdateChecklistItem dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid item ID" });
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

                var result = await _service.UpdateChecklistItemAsync(id, dto, userId);
                if (result == null)
                {
                    return NotFound(new { message = $"Checklist item with ID {id} not found" });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the checklist item", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid item ID" });
                }

                var result = await _service.DeleteChecklistItemAsync(id, userId);
                if (!result)
                {
                    return NotFound(new { message = $"Checklist item with ID {id} not found" });
                }

                return Ok(new { message = "Checklist item deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the checklist item", error = ex.Message });
            }
        }
    }
}

