using Business.Abstract;
using Entities.Dtos.WalletDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Member")]
    public class WalletController : ControllerBase
    {
        private IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost(template: "add")]
        public IActionResult Add(WalletAddDto wallet)
        {
            var result = _walletService.Add(wallet);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpGet(template: "userwalletget")]
        public IActionResult GetList(string token)
        {
            var result = _walletService.UserWalletList(token);
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
