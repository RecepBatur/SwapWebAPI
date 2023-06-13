using Business.Abstract;
using Entities.Dtos.WalletDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebAPIAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminWalletController : ControllerBase
    {
        private IWalletService _walletService;

        public AdminWalletController(IWalletService walletService)
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
        [HttpGet(template: "userwalletgetlist")]
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
        [HttpPost(template: "usergetwalletlist")]
        public IActionResult UserWalletGetList(UserWalletGetListFilterDto userWalletGetListFilterDto)
        {
            var result = _walletService.UserWalletGetList(userWalletGetListFilterDto);
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
