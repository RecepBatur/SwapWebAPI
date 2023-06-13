using Business.Abstract;
using Entities.Dtos.CompanyWalletDtos;
using Entities.Dtos.WalletDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminCompanyWalletController : ControllerBase
    {
        private readonly ICompanyWalletService _companyWalletService;

        public AdminCompanyWalletController(ICompanyWalletService companyWalletService)
        {
            _companyWalletService = companyWalletService;
        }

        //[HttpPost(template: "add")]
        //public IActionResult Add(CompanyWalletAddDto companyWalletAddDto)
        //{
        //    var result = _companyWalletService.CompanyWalletAdd(companyWalletAddDto);
        //    if (result.Success)
        //    {
        //        return Ok(result.Message);
        //    }
        //    else
        //    {
        //        return BadRequest(result.Message);
        //    }
        //}
    }
}
