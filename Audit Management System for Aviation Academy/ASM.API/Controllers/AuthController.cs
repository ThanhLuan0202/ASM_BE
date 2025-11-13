using ASM_Repositories.Models.LoginDTO;
using ASM_Services.Interfaces;
using ASM_Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _service.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });

            // Set JWT to cookie for subsequent requests
            Response.Cookies.Append("authToken", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddHours(6),
                Path = "/"
            });

            return Ok(result);
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                var result = await _service.RegisterAsync(request);
                return CreatedAtAction(nameof(Register), new { id = result.UserId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
        
    }
}
