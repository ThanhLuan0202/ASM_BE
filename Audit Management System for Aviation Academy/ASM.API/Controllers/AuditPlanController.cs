using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditPlanController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditPlanController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _auditService.GetAuditPlansAsync());

        [HttpGet("{auditId:guid}")]
        public async Task<IActionResult> GetById(Guid auditId)
        {
            var result = await _auditService.GetAuditPlanDetailsAsync(auditId);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
