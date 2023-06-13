using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Entities.Dtos.BuyOrderDtos;
using Entities.Dtos.SaleOrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleOrderController : ControllerBase
    {
        private ISalesOrderService _salesOrderService;

        public SaleOrderController(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }
        [HttpPost(template: "saleorderadd")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> SaleOrder(SaleOrderDto saleOrderDto, int parityId, string token)
        {
            var result = await _salesOrderService.SaleOrder(saleOrderDto, parityId, token);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPost(template: "getall")]
        public IActionResult SaleOrderGetList(SaleOrderGetListFilterDto saleOrderGetListFilterDto)
        {
            var result = _salesOrderService.SaleOrderGetList(saleOrderGetListFilterDto);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPost(template: "usergetall")]
        [Authorize(Roles = "Member")]
        public IActionResult UserSaleOrderList(UserSaleGetListFilterDto userSaleGetListFilterDto)
        {
            var result = _salesOrderService.UserSaleOrderList(userSaleGetListFilterDto);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPost(template: "ordercancel")]
        [Authorize(Roles = "Member")]
        public IActionResult OrderCancel(int saleOrderId, string token)
        {
            var result = _salesOrderService.OrderCancel(saleOrderId, token);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpGet("bestsaleorderlist")]
        public IActionResult BestSaleOrder()
        {
            var result = _salesOrderService.BestSaleOrderGetList();
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
