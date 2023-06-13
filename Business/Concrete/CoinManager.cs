using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos.CoinDtos;
using Entities.Dtos.SaleOrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CoinManager : ICoinService
    {
        private readonly ICoinDal _coinDal;

        public CoinManager(ICoinDal coinDal)
        {
            _coinDal = coinDal;
        }

        public IDataResult<List<CoinListDto>> CoinGetList()
        {
            var list = _coinDal.GetList();
            if (list == null)
            {
                return new ErrorDataResult<List<CoinListDto>>(Messages.ErrorList);
            }
            var coinList = new List<CoinListDto>();
            foreach (var item in list)
            {
                coinList.Add(new()
                {
                    CoinName = item.CoinName,
                });
            }
            return new SuccessDataResult<List<CoinListDto>>(coinList, Messages.List);
        }
    }
}
