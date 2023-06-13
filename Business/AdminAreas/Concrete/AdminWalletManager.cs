using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos.PagingDtos;
using Entities.Dtos.WalletDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Business.AdminAreas.Concrete
{
    public class AdminWalletManager : IWalletService
    {
        private readonly IWalletDal _walletDal;
        private readonly ITokenHelper _tokenHelper;
        private readonly IUserDal _userDal;
        private readonly ICoinDal _coinDal;

        public AdminWalletManager(IWalletDal walletDal, ITokenHelper tokenHelper, IUserDal userDal, ICoinDal coinDal)
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

        public IDataResult<UserWalletGetListPagingDto> UserWalletGetList(UserWalletGetListFilterDto userWalletGetListFilterDto)
        {
            var userWalletListDto = new List<UserWalletListDto>();
            try
            {
                var walletResult = _walletDal.GetList();
                if (walletResult != null)
                {
                    foreach (var item in walletResult)
                    {
                        userWalletListDto.Add(new UserWalletListDto
                        {
                            UserId = item.UserId,
                            Balance = item.Balance,
                            CryptoName = item.Name,
                        });
                    }

                    if (!String.IsNullOrEmpty(userWalletGetListFilterDto.Search))
                    {
                        userWalletListDto = userWalletListDto.Where(x =>
                        x.CryptoName.Contains(userWalletGetListFilterDto.Search)).ToList();
                    }
                    if (userWalletGetListFilterDto.PagingFilter.PageNumber <= 0)
                    {
                        userWalletGetListFilterDto.PagingFilter.PageNumber = 1;
                    }
                    if(userWalletGetListFilterDto.PagingFilter.PageSize <= 0)
                    {
                        userWalletGetListFilterDto.PagingFilter.PageSize = 10;
                    }

                    int total = userWalletGetListFilterDto.PagingFilter.PageSize * userWalletGetListFilterDto.PagingFilter.PageNumber;
                    var totalCount = (double)userWalletListDto.Count;
                    var totalPages = Math.Ceiling(totalCount / userWalletGetListFilterDto.PagingFilter.PageSize);
                    userWalletListDto = userWalletListDto.Skip(total).Take(userWalletGetListFilterDto.PagingFilter.PageSize).ToList();

                    var resultDto = new UserWalletGetListPagingDto()
                    {
                        Data = userWalletListDto,
                        Page = new PagingDto
                        {
                            PageSize = userWalletGetListFilterDto.PagingFilter.PageSize,
                            PageNumber = userWalletGetListFilterDto.PagingFilter.PageNumber + 1,
                            TotalItemCount = (int)totalCount,
                            TotalPageCount = (int)totalPages
                        }
                    };
                    return new SuccessDataResult<UserWalletGetListPagingDto>(resultDto, Messages.List);

                }
                return new SuccessDataResult<UserWalletGetListPagingDto>(Messages.WalletError);
            }
            catch (Exception)
            {

                return new ErrorDataResult<UserWalletGetListPagingDto>(Messages.UnknownError);
            }
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
