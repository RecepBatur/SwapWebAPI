using Business.Abstract;
using Business.AdminAreas.Abstract;
using DataAccess.Abstract;
using Entities.Dtos.CoinDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminCoinController : ControllerBase
    {
        private IAdminCoinService _adminCoinService;

        public AdminCoinController(IAdminCoinService adminCoinService)
        {
            _adminCoinService = adminCoinService;
        }

        [HttpPost(template: "add")]
        public IActionResult CoinAdd(CoinAddDto coin)
        {
            var result = _adminCoinService.CoinAdd(coin);
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
        public IActionResult GetList()
        {
            var result = _adminCoinService.CoinGetList();
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
