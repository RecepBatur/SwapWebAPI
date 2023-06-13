using Core.Utilities.Results;
using Entities.Dtos.CoinDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AdminAreas.Abstract
{
    public interface IAdminCoinService
    {
        IDataResult<bool> CoinAdd(CoinAddDto coin);
        IDataResult<List<CoinListDto>> CoinGetList();
    }
}
