using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos.CoinDtos;
using Entities.Dtos.WalletDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class WalletManager : IWalletService
    {
        private readonly IWalletDal _walletDal;
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserDal _userDal;
        private readonly ICoinDal _coinDal;

        public WalletManager(IWalletDal walletDal, ITokenHelper tokenHelper, IUserDal userDal, ICoinDal coinDal)
        {
            _walletDal = walletDal;
            _tokenHelper = tokenHelper;
            _userDal = userDal;
            _coinDal = coinDal;
        }

        public IDataResult<bool> Add(WalletAddDto wallet)
        {
            var addWallet = new Wallet
            {
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                Status = true,
                CryptoId = wallet.CryptoId,
                Name = wallet.Name,
            };
            _walletDal.Add(addWallet);

            return new SuccessDataResult<bool>(true, Messages.WalletAdded);
        }

        public IDataResult<UserWalletGetListPagingDto>UserWalletGetList(UserWalletGetListFilterDto userWalletGetListFilterDto)
        {
            throw new NotImplementedException();
        }

        public IDataResult<List<WalletListDto>> UserWalletList(string token)
        {
            var userToken = _tokenHelper.GetAuthenticatedUser(token);
            var user = _userDal.Get(x => x.Id == userToken);
            if (user == null)
            {
                return new ErrorDataResult<List<WalletListDto>>(Messages.UserNotFound);
            }

            var walletList = _walletDal.GetList(x => x.UserId == userToken);

            var wallets = new List<WalletListDto>();

            foreach (var wallet in walletList)
            {
                var coins = _coinDal.GetList(x => x.Id == wallet.CryptoId);

                wallets.Add(new WalletListDto
                {
                    Balance = wallet.Balance,
                    CoinName = wallet.Name,
                });
            }

            return new SuccessDataResult<List<WalletListDto>>(wallets, Messages.List);
        }
    }
}
