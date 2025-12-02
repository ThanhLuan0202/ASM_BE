using ASM_Repositories.Models.NotificationDTO;
using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.API.AdminControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminNotificationController : ControllerBase
    {
        private readonly INotificationService _service;

        public AdminNotificationController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewNotification>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving notifications", error = ex.Message });
            }
        }

        [HttpGet("{notificationId}")]
        public async Task<ActionResult<ViewNotification>> GetById(Guid notificationId)
        {
            try
            {
                if (notificationId == Guid.Empty)
                    return BadRequest(new { message = "Invalid NotificationId" });

                var result = await _service.GetByIdAsync(notificationId);
                if (result == null)
                    return NotFound(new { message = "Notification not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the notification", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ViewNotification>> Create([FromBody] CreateNotification dto)
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

                if (dto.UserId == Guid.Empty)
                    return BadRequest(new { message = "UserId is required" });

                if (string.IsNullOrWhiteSpace(dto.Title))
                    return BadRequest(new { message = "Title is required" });

                if (string.IsNullOrWhiteSpace(dto.Message))
                    return BadRequest(new { message = "Message is required" });

                if (string.IsNullOrWhiteSpace(dto.EntityType))
                    return BadRequest(new { message = "EntityType is required" });

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { notificationId = result.NotificationId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the notification", error = ex.Message });
            }
        }

        [HttpPut("{notificationId}")]
        public async Task<ActionResult<ViewNotification>> Update(Guid notificationId, [FromBody] UpdateNotification dto)
        {
            try
            {
                if (notificationId == Guid.Empty)
                    return BadRequest(new { message = "Invalid NotificationId" });

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                        .ToList();
                    return BadRequest(new { message = "Validation failed", errors });
                }

                if (dto.UserId == Guid.Empty)
                    return BadRequest(new { message = "UserId is required" });

                if (string.IsNullOrWhiteSpace(dto.Title))
                    return BadRequest(new { message = "Title is required" });

                if (string.IsNullOrWhiteSpace(dto.Message))
                    return BadRequest(new { message = "Message is required" });

                if (string.IsNullOrWhiteSpace(dto.EntityType))
                    return BadRequest(new { message = "EntityType is required" });

                var result = await _service.UpdateAsync(notificationId, dto);
                if (result == null)
                    return NotFound(new { message = "Notification not found" });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the notification", error = ex.Message });
            }
        }

        [HttpPut("{notificationId:guid}/read")]
        public async Task<IActionResult> MarkAsRead([FromRoute] Guid notificationId)
        {
            try
            {
                await _service.MarkAsReadAsync(notificationId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{notificationId}")]
        public async Task<ActionResult> Delete(Guid notificationId)
        {
            try
            {
                if (notificationId == Guid.Empty)
                    return BadRequest(new { message = "Invalid NotificationId" });

                var result = await _service.DeleteAsync(notificationId);
                if (!result)
                    return NotFound(new { message = "Notification not found" });

                return Ok(new { message = "Notification deleted successfully (status changed to Inactive)" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the notification", error = ex.Message });
            }
        }
    }
}

