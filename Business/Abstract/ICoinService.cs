using Core.Utilities.Results;
using Entities.Dtos.CoinDtos;
using Entities.Dtos.SaleOrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICoinService
    {
        IDataResult<List<CoinListDto>> CoinGetList();
    }
}
