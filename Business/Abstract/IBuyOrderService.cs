using Core.Utilities.Results;
using Entities.Dtos.BuyOrderDtos;
using Entities.Dtos.MarketDtos;
using Entities.Dtos.ParityDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IBuyOrderService
    {
        Task<IDataResult<bool>> BuyOrder(BuyOrderDto buyOrderDto, int parityId, string token);
        IDataResult<bool> OrderCancel(int buyOrderId, string token);
        IDataResult<List<BuyOrderListDto>> BuyOrderGetList(BuyOrderGetListFilterDto buyOrderGetListFilterDto);
        IDataResult<List<BuyOrderListDto>> UserBuyOrderList(UserGetBuyOrderListFilterDto userGetBuyOrderListFilterDto);
        IDataResult<List<MarketListDto>> GetListMarket();
        IDataResult<List<BestBuyOrderListDto>> BestBuyOrderGetList();
    }
}
