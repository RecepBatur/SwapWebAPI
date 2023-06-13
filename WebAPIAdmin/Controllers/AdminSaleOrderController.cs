using Business.Abstract;
using Entities.Dtos.SaleOrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminSaleOrderController : ControllerBase
    {
        private ISalesOrderService _salesOrderService;

        public AdminSaleOrderController(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }
        [HttpPost(template: "saleorderadd")]
        public async Task<IActionResult> BuyOrder(SaleOrderDto saleOrderDto, int parityId, string token)
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
        [HttpGet(template: "getall")]
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
        [HttpGet(template: "usergetall")]
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
