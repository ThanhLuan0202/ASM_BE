using ASM_Repositories.Models.AttachmentDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASM.API.AdminControllers
{
    [Authorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminAttachmentController : ControllerBase
    {
        private readonly IAttachmentService _service;

        public AdminAttachmentController(IAttachmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewAttachment>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving attachments", error = ex.Message });
            }
        }

        [HttpGet("{attachmentId}")]
        public async Task<ActionResult<ViewAttachment>> GetById(Guid attachmentId)
        {
            try
            {
                if (attachmentId == Guid.Empty)
                    return BadRequest(new { message = "Invalid AttachmentId" });

                var result = await _service.GetByIdAsync(attachmentId);
                if (result == null)
                    return NotFound(new { message = "Attachment not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the attachment", error = ex.Message });
            }
        }

        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<ActionResult<IEnumerable<ViewAttachment>>> GetByEntity(string entityType, Guid entityId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entityType))
                    return BadRequest(new { message = "EntityType is required" });

                if (entityId == Guid.Empty)
                    return BadRequest(new { message = "Invalid EntityId" });

                var result = await _service.GetByEntityAsync(entityType, entityId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving attachments", error = ex.Message });
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ViewAttachment>> Create([FromForm] CreateAttachment dto, IFormFile file)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "File is required" });

                if (string.IsNullOrWhiteSpace(dto.EntityType))
                    return BadRequest(new { message = "EntityType is required" });

                if (dto.EntityId == Guid.Empty)
                    return BadRequest(new { message = "EntityId is required" });

                var result = await _service.CreateAsync(dto, file, userId);
                return CreatedAtAction(nameof(GetById), new { attachmentId = result.AttachmentId }, result);
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
                return StatusCode(500, new { message = "An error occurred while creating the attachment", error = ex.Message });
            }
        }

        [HttpPut("{attachmentId}")]
        public async Task<ActionResult<ViewAttachment>> Update(Guid attachmentId, [FromBody] UpdateAttachment dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (attachmentId == Guid.Empty)
                    return BadRequest(new { message = "Invalid AttachmentId" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                var result = await _service.UpdateAsync(attachmentId, dto, userId);
                if (result == null)
                    return NotFound(new { message = "Attachment not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the attachment", error = ex.Message });
            }
        }

        [HttpPut("{attachmentId}/file")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ViewAttachment>> UpdateFile(Guid attachmentId, IFormFile file)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (attachmentId == Guid.Empty)
                    return BadRequest(new { message = "Invalid AttachmentId" });

                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "File is required" });

                var result = await _service.UpdateFileAsync(attachmentId, file, userId);
                if (result == null)
                    return NotFound(new { message = "Attachment not found" });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the file", error = ex.Message });
            }
        }

        [HttpDelete("{attachmentId}")]
        public async Task<ActionResult> Delete(Guid attachmentId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                    return Unauthorized(new { message = "Invalid or missing UserId in token." });

                if (attachmentId == Guid.Empty)
                    return BadRequest(new { message = "Invalid AttachmentId" });

                var result = await _service.DeleteAsync(attachmentId, userId);
                if (!result)
                    return NotFound(new { message = "Attachment not found" });

                return Ok(new { message = "Attachment deleted successfully (status changed to Inactive)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the attachment", error = ex.Message });
            }
        }
    }
}

