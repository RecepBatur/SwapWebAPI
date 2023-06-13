using Business.Abstract;
using Entities.Concrete;
using Entities.Dtos.ParityDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParityController : ControllerBase
    {
        private IParityService _parityService;

        public ParityController(IParityService parityService)
        {
            _parityService = parityService;
        }
        [HttpGet(template: "getall")]
        public async Task<IActionResult> ParityGetList()
        {
            var result = await _parityService.ParityGetList();
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
