using Business.Abstract;
using Entities.Dtos.ParityDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminParityController : ControllerBase
    {
        private IParityService _parityService;

        public AdminParityController(IParityService parityService)
        {
            _parityService = parityService;
        }
        [HttpPost(template: "add")]
        public IActionResult ParityAdd(ParityAddedDto parity)
        {
            var result = _parityService.ParityAdd(parity);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
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
