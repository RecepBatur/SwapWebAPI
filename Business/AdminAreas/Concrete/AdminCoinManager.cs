using Business.Abstract;
using Business.AdminAreas.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities.Concrete;
using Entities.Dtos.CoinDtos;
using Entities.Dtos.CompanyWalletDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Business.AdminAreas.Concrete
{
    public class AdminCoinManager : IAdminCoinService
    {
        private readonly ICoinDal _coinDal;
        private readonly ICompanyWalletDal _companyWalletDal;

        public AdminCoinManager(ICoinDal coinDal, ICompanyWalletDal companyWalletDal)
        {
            _coinDal = coinDal;
            _companyWalletDal = companyWalletDal;
        }

        public IDataResult<bool> CoinAdd(CoinAddDto coin)
        {
            var coinAdd = new Coin
            {
                CoinName = coin.CoinName,
                Status = true,

            };
            _coinDal.Add(coinAdd);

            var cmWallet = new CompanyWallet
            {
                Balance = 0,
                CryptoId = coinAdd.Id,
                Status = true,

            };
            _companyWalletDal.Add(cmWallet);

        

            return new SuccessDataResult<bool>(true, Messages.CoinAdded);
        }

        public IDataResult<List<CoinListDto>> CoinGetList()
        {
            var list = _coinDal.GetList();
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
