using Business.Abstract;
using Entities.Dtos.BuyOrderDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyOrderController : ControllerBase
    {
        private IBuyOrderService _buyOrderService;

        public BuyOrderController(IBuyOrderService buyOrderService)
        {
            _buyOrderService = buyOrderService;
        }

        [HttpPost(template: "buyorderadd")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> BuyOrder(BuyOrderDto buyOrderDto, int parityId, string token)
        {
            var result = await _buyOrderService.BuyOrder(buyOrderDto, parityId, token);
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
        public IActionResult BuyOrderGetList(BuyOrderGetListFilterDto buyOrderGetListFilterDto)
        {
            var result = _buyOrderService.BuyOrderGetList(buyOrderGetListFilterDto);
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
        public IActionResult UserBuyOrderList(UserGetBuyOrderListFilterDto userGetBuyOrderListFilterDto)
        {
            var result = _buyOrderService.UserBuyOrderList(userGetBuyOrderListFilterDto);
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
        public IActionResult OrderCancel(int buyOrderId, string token)
        {
            var result = _buyOrderService.OrderCancel(buyOrderId, token);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpGet("marketlist")]
        public IActionResult MarketGetList()
        {
            var result = _buyOrderService.GetListMarket();
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
            
        }
        [HttpGet("bestbuyorderlist")]
        public IActionResult BestBuyOrder()
        {
            var result = _buyOrderService.BestBuyOrderGetList();
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
