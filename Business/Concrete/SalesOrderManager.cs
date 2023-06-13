using Business.Abstract;
using Business.Constants;
using Business.Enums;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities.Concrete;
using Entities.Dtos.BuyOrderDtos;
using Entities.Dtos.PagingDtos;
using Entities.Dtos.ParityDtos;
using Entities.Dtos.SaleOrderDtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class SalesOrderManager : ISalesOrderService
    {
        private readonly ISalesOrderDal _salesOrderDal;
        private readonly IBuyOrderDal _buyOrderDal;
        private readonly IUserDal _userDal;
        private readonly IWalletDal _walletDal;
        private readonly ICompanyWalletDal _companyWalletDal;
        private readonly IParityDal _parityDal;
        private readonly ITokenHelper _tokenHelper;
        public SalesOrderManager(ISalesOrderDal salesOrderDal, IUserDal userDal, IWalletDal walletDal, ITokenHelper tokenHelper, IBuyOrderDal buyOrderDal, IParityDal parityDal, ICompanyWalletDal companyWalletDal)
        {
            _salesOrderDal = salesOrderDal;
            _userDal = userDal;
            _walletDal = walletDal;
            _tokenHelper = tokenHelper;
            _buyOrderDal = buyOrderDal;
            _parityDal = parityDal;
            _companyWalletDal = companyWalletDal;
        }

        public async Task<IDataResult<bool>> SaleOrder(SaleOrderDto saleOrderDto, int parityId, string token)
        {
            var userToken = _tokenHelper.GetAuthenticatedUser(token);
            var users = _userDal.Get(x => x.Id == userToken);
            if (users == null)
            {
                return new ErrorDataResult<bool>(Messages.UserNotFound);
            }
            var parity = _parityDal.Get(x => x.Id == parityId);
            var decreasingCoin = _walletDal.Get(x => x.UserId == users.Id && x.CryptoId == parity.First); //azalan
            if (decreasingCoin == null)
            {
                return new ErrorDataResult<bool>(false, Messages.Coin);
            }
            var growingCoin = _walletDal.Get(x => x.UserId == users.Id && x.CryptoId == parity.Second); //artan
            if (growingCoin == null)
            {
                return new ErrorDataResult<bool>(false, Messages.Coin);
            }
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://api.binance.com/api/v3/ticker/price?symbol={parity.Symbol}");
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ParityDto>(content);

                decimal minPrice = 0;
                decimal maxPrice = 0;

                if (result.Price < 100)
                {
                    minPrice = result.Price - 5;
                    maxPrice = result.Price + 5;
                }
                else if (result.Price >= 100 && result.Price < 1000)
                {
                    minPrice = result.Price - 20;
                    maxPrice = result.Price + 20;
                }
                else if (result.Price >= 1000 && result.Price < 10000)
                {
                    minPrice = result.Price - 80;
                    maxPrice = result.Price + 80;
                }
                else if (result.Price >= 10000 && result.Price < 100000)
                {
                    minPrice = result.Price - 320;
                    maxPrice = result.Price + 320;
                }
                else if (result.Price >= 100000 && result.Price < 1000000)
                {
                    minPrice = result.Price - 1240;
                    maxPrice = result.Price + 1240;
                }
                if (saleOrderDto.Price < minPrice || saleOrderDto.Price > maxPrice)
                {
                    return new ErrorDataResult<bool>(Messages.Price);
                }

                decimal totalAmount = saleOrderDto.Amount;
                decimal userAmount = saleOrderDto.Amount;
                decimal totalPrice = saleOrderDto.Amount * saleOrderDto.Price;
                decimal totalSalePrice = saleOrderDto.Amount * saleOrderDto.Price;

                if (decreasingCoin.Balance < totalSalePrice)
                {
                    return new ErrorDataResult<bool>(Messages.InsufficientBalance);
                }
                var buyOrders = _buyOrderDal.GetList(x => x.ParityId == parityId && x.Price <= saleOrderDto.Price && x.Status == 1)
                                        .OrderBy(x => x.Price);

                foreach (var buyOrder in buyOrders)
                {
                    if (totalAmount == 0) // tüm emirler tamamlandıysa işlemi bitir
                    {
                        break;

                    }

                    var buyAmount = Math.Min(buyOrder.Amount, totalAmount); // alınacak miktarı belirle
                    var buyPrice = buyOrder.Price;

                    totalPrice += buyAmount * buyPrice; // toplam fiyata ekle

                    // alınacak miktarı emirden düş ve emirin toplam fiyatını güncelle
                    buyOrder.Amount -= buyAmount;
                    buyOrder.TotalPrice = buyOrder.Amount * buyPrice;

                    if (buyOrder.Amount == 0) // emir tamamlandıysa emrin durumunu güncelle
                    {
                        buyOrder.Status = 0;

                    }

                    totalAmount -= buyAmount; // alınacak miktarı güncelle

                    _buyOrderDal.Update(buyOrder); // emirin güncel haliyle veritabanını güncelle

                }

                decimal commission = totalSalePrice * parity.Commission / 100;
                decimal netPrice = totalSalePrice - commission;

                //kaybedilen parayı hesaplayıp kullanıcı cüzdanından düşüyoruz.
                decimal newBalance = growingCoin.Balance + userAmount;
                decreasingCoin.Balance -= totalSalePrice + commission;
                growingCoin.Balance = newBalance;

                _walletDal.Update(growingCoin);
                _walletDal.Update(decreasingCoin);

                //şirket cüzdanına komisyonu ekliyoruz.
                var companyWallets = _companyWalletDal.Get(x => x.CryptoId == parity.First);
                companyWallets.Balance += commission;
                _companyWalletDal.Update(companyWallets);

                //Satış emrini ekliyoruz db'ye.

                SalesOrder saleorderAdd = new SalesOrder();
                {
                    saleorderAdd.ParityId = parityId;
                    saleorderAdd.ParityName = parity.Symbol;
                    saleorderAdd.UserId = userToken;
                    saleorderAdd.Price = saleOrderDto.Price;
                    saleorderAdd.Amount = saleOrderDto.Amount;
                    saleorderAdd.NetPrice = netPrice;
                    saleorderAdd.Commission = commission;
                    saleorderAdd.TotalPrice = totalSalePrice;
                    saleorderAdd.Status = 1;
                    saleorderAdd.CreatedDate = DateTime.Now;
                    saleorderAdd.ModifiedDate = DateTime.Now;

                    foreach (var buyOrder in buyOrders)
                    {
                        if (buyOrder.Status == 0)
                        {
                            saleorderAdd.BuyerId = buyOrder.UserId;
                            saleorderAdd.BuyerOrderId = buyOrder.Id;
                            saleorderAdd.Status = 0;
                        }
                        else
                        {
                            saleorderAdd.BuyerId = 0;
                        }
                    }

                }
                _salesOrderDal.Add(saleorderAdd);


            }

            return new SuccessDataResult<bool>(Messages.SaleOrder);
        }

        public IDataResult<List<SaleOrderListDto>> SaleOrderGetList(SaleOrderGetListFilterDto saleOrderGetListFilterDto)
        {
            try
            {
                if (saleOrderGetListFilterDto.PagingFilter.PageNumber <= 0)
                {
                    saleOrderGetListFilterDto.PagingFilter.PageNumber = 1;
                }
                if (saleOrderGetListFilterDto.PagingFilter.PageSize <= 0)
                {
                    saleOrderGetListFilterDto.PagingFilter.PageSize = 10;
                }

                var totalCount = _salesOrderDal.GetList().Count;

                var saleOrders = _salesOrderDal.GetList(x => x.Status == 1);
                if (saleOrders.Count == 0)
                {
                    return new ErrorDataResult<List<SaleOrderListDto>>(Messages.SaleOrderError);
                }

                var parities = _parityDal.GetList().ToList();

                var list = saleOrders
                    .Where(saleOrder => parities.Any(parity => parity.Id == saleOrder.ParityId))
                    .Select(saleOrder => new { saleOrder, parity = parities.FirstOrDefault(parity => parity.Id == saleOrder.ParityId) })
                    .Where(x => x.saleOrder.ParityName.Trim().ToLower().Contains($"{saleOrderGetListFilterDto.Search.Trim().ToLower()}"))
                    .OrderByDescending(x => x.saleOrder.Price)
                    .Skip((saleOrderGetListFilterDto.PagingFilter.PageNumber - 1) * saleOrderGetListFilterDto.PagingFilter.PageSize)
                    .Take(saleOrderGetListFilterDto.PagingFilter.PageSize)
                    .ToList();

                if (list.Count == 0)
                {
                    return new ErrorDataResult<List<SaleOrderListDto>>(Messages.SaleOrderError);
                }

                var saleOrderList = list.Select(item => new SaleOrderListDto
                {
                    UserId = item.saleOrder.UserId,
                    BuyerId = item.saleOrder.BuyerId.Value,
                    BuyerOrderId = item.saleOrder.BuyerOrderId,
                    //ParityId = item.ParityId,
                    ParityName = item.saleOrder.ParityName,
                    Price = item.saleOrder.Price,
                    Amount = item.saleOrder.Amount,
                    NetPrice = item.saleOrder.NetPrice,
                    Commission = item.saleOrder.Commission,
                    TotalPrice = item.saleOrder.TotalPrice,
                    Status = ((BaseStatus)item.saleOrder.Status).ToString(),
                    CreatedDate = item.saleOrder.CreatedDate,
                    ModifiedDate = item.saleOrder.ModifiedDate

                })
                    .ToList();

                var pagination = new PagingDto
                {
                    PageNumber = saleOrderGetListFilterDto.PagingFilter.PageNumber,
                    PageSize = saleOrderGetListFilterDto.PagingFilter.PageSize,
                    TotalItemCount = totalCount,
                    TotalPageCount = (int)Math.Ceiling((double)totalCount / saleOrderGetListFilterDto.PagingFilter.PageSize),
                };

                return new SuccessDataResult<List<SaleOrderListDto>>(saleOrderList, Messages.List);
            }
            catch (Exception e)
            {

                return new ErrorDataResult<List<SaleOrderListDto>>(Messages.SaleOrderError);
            }

        }

        public IDataResult<bool> OrderCancel(int saleOrderId, string token)
        {
            var saleorder = _salesOrderDal.Get(x => x.Id == saleOrderId);
            if (saleorder != null)
            {
                if (saleorder.Status == (int)BaseStatus.Waiting)
                {
                    var userToken = _tokenHelper.GetAuthenticatedUser(token);
                    var users = _userDal.Get(x => x.Id == userToken);
                    if (users == null)
                    {
                        return new ErrorDataResult<bool>(Messages.UserNotFound);
                    }
                    var parity = _parityDal.Get(x => x.Id == saleorder.ParityId);
                    if (parity == null)
                    {
                        return new ErrorDataResult<bool>(false, Messages.ParityError);
                    }
                    var decreasingCoin = _walletDal.Get(x => x.UserId == users.Id && x.CryptoId == parity.First); //azalan
                    if (decreasingCoin == null)
                    {
                        return new ErrorDataResult<bool>(false, Messages.Coin);
                    }

                    //kullanıcıya iade işlemi
                    decreasingCoin.Balance += saleorder.TotalPrice + saleorder.Commission;
                    _walletDal.Update(decreasingCoin);

                    //şirket cüzdanından komisyonu çıkartıyoruz
                    var companyWallets = _companyWalletDal.Get(x => x.CryptoId == parity.First);
                    if (companyWallets == null)
                    {
                        return new ErrorDataResult<bool>(false, Messages.WalletError);
                    }
                    companyWallets.Balance += saleorder.Commission;
                    _companyWalletDal.Update(companyWallets);

                    saleorder.Status = (int)BaseStatus.Canceled;
                    _salesOrderDal.Update(saleorder);
                    return new SuccessDataResult<bool>(true, Messages.SaleOrderCancel);
                }
                else
                {
                    return new ErrorDataResult<bool>(false, Messages.OrderNotFound);
                }
            }
            else
            {
                return new ErrorDataResult<bool>(false, Messages.OrderNotFound);

            }
        }

        public IDataResult<List<SaleOrderListDto>> UserSaleOrderList(UserSaleGetListFilterDto userSaleGetListFilterDto)
        {
            try
            {
                if (userSaleGetListFilterDto.PagingFilter.PageNumber <= 0)
                {
                    userSaleGetListFilterDto.PagingFilter.PageNumber = 1;
                }
                if (userSaleGetListFilterDto.PagingFilter.PageSize <= 0)
                {
                    userSaleGetListFilterDto.PagingFilter.PageSize = 10;
                }

                var userToken = _tokenHelper.GetAuthenticatedUser(userSaleGetListFilterDto.Token);
                var user = _userDal.Get(x => x.Id == userToken);
                if (user == null)
                {
                    return new ErrorDataResult<List<SaleOrderListDto>>(Messages.UserNotFound);
                }
                var saleOrders = _salesOrderDal.GetList().Where(x => x.UserId == user.Id).ToList();
                if (saleOrders.Count == 0)
                {
                    return new ErrorDataResult<List<SaleOrderListDto>>(Messages.UserOrdersNotFound);
                }

                var totalCount = _salesOrderDal.GetList(x => x.UserId == userToken).Count;


                var parities = _parityDal.GetList().ToList();

                var list = saleOrders
                    .Where(saleOrder => parities.Any(parity => parity.Id == saleOrder.ParityId))
                    .Select(saleOrder => new { saleOrder, parity = parities.FirstOrDefault(parity => parity.Id == saleOrder.ParityId) })
                    .Where(x => x.saleOrder.ParityName.Trim().ToLower().Contains($"{userSaleGetListFilterDto.Search.Trim().ToLower()}"))
                    .OrderByDescending(x => x.saleOrder.Price)
                    .Skip((userSaleGetListFilterDto.PagingFilter.PageNumber - 1) * userSaleGetListFilterDto.PagingFilter.PageSize)
                    .Take(userSaleGetListFilterDto.PagingFilter.PageSize)
                    .ToList();

                if (list.Count == 0)
                {
                    return new ErrorDataResult<List<SaleOrderListDto>>(Messages.SaleOrderError);
                }

                var saleOrderList = list.Select(item => new SaleOrderListDto
                {
                    UserId = item.saleOrder.UserId,
                    BuyerId = item.saleOrder.BuyerId.Value,
                    BuyerOrderId = item.saleOrder.BuyerOrderId,
                    ParityName = item.saleOrder.ParityName,
                    Price = item.saleOrder.Price,
                    Amount = item.saleOrder.Amount,
                    NetPrice = item.saleOrder.NetPrice,
                    Commission = item.saleOrder.Commission,
                    TotalPrice = item.saleOrder.TotalPrice,
                    Status = ((BaseStatus)item.saleOrder.Status).ToString(),
                    CreatedDate = item.saleOrder.CreatedDate,
                    ModifiedDate = item.saleOrder.ModifiedDate

                })
                    .ToList();

                var pagination = new PagingDto
                {
                    PageNumber = userSaleGetListFilterDto.PagingFilter.PageNumber,
                    PageSize = userSaleGetListFilterDto.PagingFilter.PageSize,
                    TotalItemCount = totalCount,
                    TotalPageCount = (int)Math.Ceiling((double)totalCount / userSaleGetListFilterDto.PagingFilter.PageSize),
                };

                return new SuccessDataResult<List<SaleOrderListDto>>(saleOrderList, Messages.List);
            }
            catch (Exception e)
            {

                return new ErrorDataResult<List<SaleOrderListDto>>(Messages.SaleOrderError);
            }

        }

        public IDataResult<List<BestSaleOrderListDto>> BestSaleOrderGetList()
        {
            try
            {
                var saleOrder = _salesOrderDal.GetList(x => x.Status == 1).OrderByDescending(x => x.Price).Take(10);
                var dto = new List<BestSaleOrderListDto>();
                if (saleOrder != null)
                {
                    foreach (var item in saleOrder)
                    {
                        dto.Add(new BestSaleOrderListDto
                        {
                            Price = item.Price,
                            TotalPrice = item.TotalPrice,
                            Amount = item.Amount,
                        });
                    }

                    return new SuccessDataResult<List<BestSaleOrderListDto>>(dto, Messages.List);
                }
                return new ErrorDataResult<List<BestSaleOrderListDto>>(dto, Messages.ErrorList);
            }
            catch (Exception)
            {

                return new ErrorDataResult<List<BestSaleOrderListDto>>(Messages.UnknownError);
            }

        }
    }
}
