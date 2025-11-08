using ASM_Repositories.Models.AttachmentEntityTypeDTO;
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
    public class AdminAttachmentEntityTypeController : ControllerBase
    {
        private readonly IAttachmentEntityTypeService _service;

        public AdminAttachmentEntityTypeController(IAttachmentEntityTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewAttachmentEntityType>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving attachment entity types", error = ex.Message });
            }
        }

        [HttpGet("{entityType}")]
        public async Task<ActionResult<ViewAttachmentEntityType>> GetById(string entityType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entityType))
                    return BadRequest(new { message = "EntityType is required" });

                var result = await _service.GetByIdAsync(entityType);
                if (result == null)
                    return NotFound(new { message = "AttachmentEntityType not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the attachment entity type", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewAttachmentEntityType>> Create([FromBody] CreateAttachmentEntityType dto)
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

                if (string.IsNullOrWhiteSpace(dto.EntityType))
                    return BadRequest(new { message = "EntityType is required" });

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { entityType = result.EntityType }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the attachment entity type", error = ex.Message });
            }
        }

        [HttpPut("{entityType}")]
        public async Task<ActionResult<ViewAttachmentEntityType>> Update(string entityType, [FromBody] UpdateAttachmentEntityType dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entityType))
                    return BadRequest(new { message = "EntityType is required" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (string.IsNullOrWhiteSpace(dto.EntityType))
                    return BadRequest(new { message = "EntityType is required" });

                var result = await _service.UpdateAsync(entityType, dto);
                if (result == null)
                    return NotFound(new { message = "AttachmentEntityType not found" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the attachment entity type", error = ex.Message });
            }
        }

        [HttpDelete("{entityType}")]
        public async Task<ActionResult> Delete(string entityType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(entityType))
                    return BadRequest(new { message = "EntityType is required" });

                var result = await _service.DeleteAsync(entityType);
                if (!result)
                    return NotFound(new { message = "AttachmentEntityType not found" });

                return Ok(new { message = "AttachmentEntityType deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the attachment entity type", error = ex.Message });
            }
        }
    }
}

