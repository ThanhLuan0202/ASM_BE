using ASM_Repositories.Entities;
using ASM_Repositories.Models.FindingDTO;
using ASM_Services.Interfaces.SQAStaffInterfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FindingsController : ControllerBase
    {
        private readonly IFindingService _service;
        public FindingsController(IFindingService service)
        {
            _service = service;
        }
        // GET: api/<FindingsController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ViewFinding>>> Get()
        {
            return Ok(await _service.GetAllFinding());
        }

        // GET api/<FindingsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FindingsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FindingsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FindingsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
