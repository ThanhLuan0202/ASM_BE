using ASM_Repositories.Models.AccessGrantDTO;
using ASM_Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccessGrantController : ControllerBase
    {
        private readonly IAccessGrantService _service;

        public AccessGrantController(IAccessGrantService service)
        {
            _service = service;
        }

        [HttpPost("issue")]
        public async Task<IActionResult> Issue([FromBody] IssueAccessGrantRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Request body is required" });
                }

                if (request.ValidFrom >= request.ValidTo)
                {
                    return BadRequest(new { message = "ValidFrom must be before ValidTo" });
                }

                var response = await _service.IssueAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while issuing access grant", error = ex.Message });
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAccessGrants([FromQuery] Guid? auditId = null, [FromQuery] int? deptId = null)
        {
            try
            {
                var grants = await _service.GetAccessGrantsAsync(auditId, deptId);
                return Ok(grants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving access grants", error = ex.Message });
            }
        }

        /// <summary>
        /// Verify QR token
        /// </summary>
        [HttpGet("verify/{qrToken}")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyQrToken(string qrToken, [FromQuery] string verifyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(qrToken))
                {
                    return BadRequest(new { message = "QR token is required" });
                }

                if (string.IsNullOrEmpty(verifyCode))
                {
                    return BadRequest(new { message = "Verify code is required" });
                }

                var result = await _service.VerifyQrTokenAsync(qrToken, verifyCode);
                
                if (!result.IsValid)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while verifying QR token", error = ex.Message });
            }
        }
    }
}

