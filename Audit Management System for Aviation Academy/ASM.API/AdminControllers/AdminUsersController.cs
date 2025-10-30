using ASM_Services.Interfaces.AdminInterfaces;
using ASM_Services.Interfaces.AdminInterfaces.AdminServices;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM.API.AdminControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUsersController : ControllerBase
    {
        private readonly IUsersService _service;

        public AdminUsersController(IUsersService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllUsersAsync();
            return Ok(result);
        }

        
    }
}

