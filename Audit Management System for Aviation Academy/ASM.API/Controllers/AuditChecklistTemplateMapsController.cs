using ASM_Repositories.Models.AuditChecklistTemplateMapDTO;
using ASM_Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditChecklistTemplateMapsController : ControllerBase
    {
        private readonly IAuditChecklistTemplateMapService _service;

        public AuditChecklistTemplateMapsController(IAuditChecklistTemplateMapService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewAuditChecklistTemplateMap>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{auditId:guid}/{templateId:guid}")]
        public async Task<ActionResult<ViewAuditChecklistTemplateMap>> Get(Guid auditId, Guid templateId)
        {
            var result = await _service.GetAsync(auditId, templateId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ViewAuditChecklistTemplateMap>> Create([FromBody] CreateAuditChecklistTemplateMap dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get),
                new { auditId = result.AuditId, templateId = result.TemplateId },
                result);
        }

        [HttpPut("{auditId:guid}/{templateId:guid}")]
        public async Task<ActionResult<ViewAuditChecklistTemplateMap>> Update(
            Guid auditId,
            Guid templateId,
            [FromBody] UpdateAuditChecklistTemplateMap dto)
        {
            var result = await _service.UpdateAsync(auditId, templateId, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{auditId:guid}/{templateId:guid}")]
        public async Task<IActionResult> Delete(Guid auditId, Guid templateId)
        {
            await _service.DeleteAsync(auditId, templateId);
            return NoContent();
        }
    }
}
