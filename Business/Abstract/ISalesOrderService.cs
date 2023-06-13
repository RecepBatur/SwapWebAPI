using Core.Utilities.Results;
using Entities.Dtos.BuyOrderDtos;
using Entities.Dtos.SaleOrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ISalesOrderService
    {
        Task<IDataResult<bool>> SaleOrder(SaleOrderDto saleOrderDto, int parityId, string token);
        IDataResult<List<SaleOrderListDto>> SaleOrderGetList(SaleOrderGetListFilterDto saleOrderGetListFilterDto);
        IDataResult<List<SaleOrderListDto>> UserSaleOrderList(UserSaleGetListFilterDto userSaleGetListFilterDto);
        IDataResult<bool> OrderCancel(int saleOrderId, string token);
        IDataResult<List<BestSaleOrderListDto>> BestSaleOrderGetList();
    }
}
