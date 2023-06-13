using Business.Abstract;
using Business.Constants;
using Business.Enums;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos.BuyOrderDtos;
using Entities.Dtos.MarketDtos;
using Entities.Dtos.PagingDtos;
using Entities.Dtos.ParityDtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AdminAreas.Concrete
{
    public class AdminBuyOrderManager : IBuyOrderService
    {
        private readonly IBuyOrderDal _buyOrderDal;
        private readonly ISalesOrderDal _salesOrderDal;
        private readonly IParityDal _parityDal;
        private readonly IUserDal _userDal;
        private readonly IWalletDal _walletDal;
        private readonly ICompanyWalletDal _companyWalletDal;
        private readonly ITokenHelper _tokenHelper;

        public AdminBuyOrderManager(IBuyOrderDal buyOrderDal, ISalesOrderDal salesOrderDal, IParityDal parityDal, IUserDal userDal, IWalletDal walletDal, ICompanyWalletDal companyWalletDal, ITokenHelper tokenHelper)
        {
            _buyOrderDal = buyOrderDal;
            _salesOrderDal = salesOrderDal;
            _parityDal = parityDal;
            _userDal = userDal;
            _walletDal = walletDal;
            _companyWalletDal = companyWalletDal;
            _tokenHelper = tokenHelper;
        }

        public IDataResult<List<BestBuyOrderListDto>> BestBuyOrderGetList()
        {
            try
            {
                var buyOrder = _buyOrderDal.GetList(x => x.Status == 1).OrderByDescending(x => x.Price).Take(10);
                var dto = new List<BestBuyOrderListDto>();
                if (buyOrder != null)
                {
                    foreach (var item in buyOrder)
                    {
                        dto.Add(new BestBuyOrderListDto
                        {
                            TotalPrice = item.TotalPrice,
                            Price = item.Price,
                            Amount = item.Amount,
                        });
                    }

                    return new SuccessDataResult<List<BestBuyOrderListDto>>(dto, Messages.List);
                }
                return new ErrorDataResult<List<BestBuyOrderListDto>>(dto, Messages.ErrorList);

            }
            catch (Exception)
            {

                return new ErrorDataResult<List<BestBuyOrderListDto>>(Messages.UnknownError);
            }

        }

        public async Task<IDataResult<bool>> BuyOrder(BuyOrderDto buyOrderDto, int parityId, string token)
        {
            var userToken = _tokenHelper.GetAuthenticatedUser(token);
            var users = _userDal.Get(x => x.Id == userToken);

            if (users == null)
            {
                return new ErrorDataResult<bool>(Messages.UserNotFound);
            }
            var parity = _parityDal.Get(x => x.Id == parityId);
            var decreasingCoin = _walletDal.Get(x => x.UserId == users.Id && x.CryptoId == parity.Second); //azalan
            if (decreasingCoin == null)
            {
                return new ErrorDataResult<bool>(false, Messages.Coin);
            }
            var growingCoin = _walletDal.Get(x => x.UserId == users.Id && x.CryptoId == parity.First); //artan
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
                if (buyOrderDto.Price < minPrice || buyOrderDto.Price > maxPrice)
                {
                    return new ErrorDataResult<bool>(Messages.Price);
                }

                decimal totalAmount = buyOrderDto.Amount;
                decimal userAmount = buyOrderDto.Amount;
                decimal totalPrice = buyOrderDto.Amount * buyOrderDto.Price;
                decimal totalBuyPrice = buyOrderDto.Amount * buyOrderDto.Price;



                if (decreasingCoin.Balance < totalBuyPrice)
                {
                    return new ErrorDataResult<bool>(Messages.InsufficientBalance);
                }
                var sellOrders = _salesOrderDal.GetList(x => x.ParityId == parityId && x.Price <= buyOrderDto.Price && x.Status == 1)
                                        .OrderBy(x => x.Price);

                foreach (var sellOrder in sellOrders)
                {
                    if (totalAmount == 0) // tüm emirler tamamlandıysa işlemi bitir
                    {
                        break;
                    }

                    var sellAmount = Math.Min(sellOrder.Amount, totalAmount); // alınacak miktarı belirle
                    var sellPrice = sellOrder.Price;

                    totalPrice += sellAmount * sellPrice; // toplam fiyata ekle

                    // alınacak miktarı emirden düş ve emirin toplam fiyatını güncelle
                    sellOrder.Amount -= sellAmount;
                    sellOrder.TotalPrice = sellOrder.Amount * sellPrice;

                    if (sellOrder.Amount == 0) // emir tamamlandıysa emrin durumunu güncelle
                    {
                        sellOrder.Status = 0;
                    }



                    totalAmount -= sellAmount; // alınacak miktarı güncelle


                    _salesOrderDal.Update(sellOrder); // emirin güncel haliyle veritabanını güncelle
                }

                decimal commission = totalBuyPrice * parity.Commission / 100;
                decimal netPrice = totalBuyPrice - commission;


                //kaybedilen parayı hesaplayıp kullanıcı cüzdanından düşüyoruz.
                decimal newBalance = growingCoin.Balance + userAmount;
                decreasingCoin.Balance -= totalBuyPrice + commission;
                growingCoin.Balance = newBalance;

                _walletDal.Update(growingCoin);
                _walletDal.Update(decreasingCoin);

                //şirket cüzdanına komisyonu ekliyoruz.
                var companyWallets = _companyWalletDal.Get(x => x.CryptoId == parity.Second);
                companyWallets.Balance += commission;
                _companyWalletDal.Update(companyWallets);

                //Alış emrini ekliyoruz db'ye.
                BuyOrder buyorderAdd = new BuyOrder();
                {
                    buyorderAdd.ParityId = parityId;
                    buyorderAdd.ParityName = parity.Symbol;
                    buyorderAdd.UserId = userToken;
                    buyorderAdd.Price = buyOrderDto.Price;
                    buyorderAdd.Amount = buyOrderDto.Amount;
                    buyorderAdd.NetPrice = netPrice;
                    buyorderAdd.Commission = commission;
                    buyorderAdd.TotalPrice = totalBuyPrice;
                    buyorderAdd.Status = 1;
                    buyorderAdd.CreatedDate = DateTime.Now;
                    buyorderAdd.ModifiedDate = DateTime.Now;

                    foreach (var sellOrder in sellOrders)
                    {
                        if (sellOrder.Status == 0)
                        {
                            buyorderAdd.SellerId = sellOrder.UserId;
                            buyorderAdd.SellerOrderId = sellOrder.Id;
                            buyorderAdd.Status = 0;
                        }
                        else
                        {
                            buyorderAdd.SellerId = null;
                        }
                    }
                }
                _buyOrderDal.Add(buyorderAdd);


                return new SuccessDataResult<bool>(Messages.BuyOrder);


                //if (totalAmount > 0) // eşleşen emir yoksa 
                //{
                //    return new ErrorDataResult<bool>(Messages.BuyError);
                //}
            }
        }
        public IDataResult<List<BuyOrderListDto>> BuyOrderGetList(BuyOrderGetListFilterDto buyOrderGetListFilterDto)
        {
            try
            {
                if (buyOrderGetListFilterDto.PagingFilter.PageNumber <= 0)
                {
                    buyOrderGetListFilterDto.PagingFilter.PageNumber = 1;
                }
                if (buyOrderGetListFilterDto.PagingFilter.PageSize <= 0)
                {
                    buyOrderGetListFilterDto.PagingFilter.PageSize = 10;
                }

                var totalCount = _buyOrderDal.GetList().Count;

                var buyOrders = _buyOrderDal.GetList(x => x.Status == 1).ToList();
                if (buyOrders.Count == 0)
                {
                    return new ErrorDataResult<List<BuyOrderListDto>>(Messages.BuyOrderError);
                }
                var parities = _parityDal.GetList().ToList();

                var list = buyOrders
                            .Where(buyOrder => parities.Any(parity => parity.Id == buyOrder.ParityId))
                            .Select(buyOrder => new { buyOrder, parity = parities.FirstOrDefault(parity => parity.Id == buyOrder.ParityId) })
                            .Where(x => x.buyOrder.ParityName.Trim().ToLower().Contains($"{buyOrderGetListFilterDto.Search.Trim().ToLower()}"))
                            .OrderByDescending(x => x.buyOrder.Price)
                            .Skip((buyOrderGetListFilterDto.PagingFilter.PageNumber - 1) * buyOrderGetListFilterDto.PagingFilter.PageSize)
                            .Take(buyOrderGetListFilterDto.PagingFilter.PageSize)
                            .ToList();

                if (list.Count == 0)
                {
                    return new ErrorDataResult<List<BuyOrderListDto>>(Messages.BuyOrderError);
                }

                var buyOrderList = list.Select(item => new BuyOrderListDto
                {
                    UserId = item.buyOrder.UserId,
                    SellerId = item.buyOrder.SellerId.Value,
                    SellerOrderId = item.buyOrder.SellerOrderId,
                    ParityName = item.buyOrder.ParityName,
                    Price = item.buyOrder.Price,
                    Amount = item.buyOrder.Amount,
                    NetPrice = item.buyOrder.NetPrice,
                    Commission = item.buyOrder.Commission,
                    TotalPrice = item.buyOrder.TotalPrice,
                    Status = ((BaseStatus)item.buyOrder.Status).ToString(),
                    CreatedDate = item.buyOrder.CreatedDate,
                    ModifiedDate = item.buyOrder.ModifiedDate
                })
                            .ToList();

                var pagination = new PagingDto
                {
                    PageNumber = buyOrderGetListFilterDto.PagingFilter.PageNumber,
                    PageSize = buyOrderGetListFilterDto.PagingFilter.PageSize,
                    TotalItemCount = totalCount,
                    TotalPageCount = (int)Math.Ceiling((double)totalCount / buyOrderGetListFilterDto.PagingFilter.PageSize),
                };

                return new SuccessDataResult<List<BuyOrderListDto>>(buyOrderList, Messages.List);
            }
            catch (Exception e)
            {

                return new ErrorDataResult<List<BuyOrderListDto>>(Messages.ErrorList);
            }

        }

        public IDataResult<List<MarketListDto>> GetListMarket()
        {
            try
            {
                var buyOrder = _buyOrderDal.GetList(x => x.Status == 0);
                if (buyOrder.Count == 0)
                {
                    return new ErrorDataResult<List<MarketListDto>>(Messages.BuyOrderError);
                }
                var saleOrder = _salesOrderDal.GetList(x => x.Status == 0);
                if (saleOrder.Count == 0)
                {
                    return new ErrorDataResult<List<MarketListDto>>(Messages.SaleOrderError);
                }

                List<MarketListDto> data = new List<MarketListDto>();

                foreach (var item in saleOrder)
                {
                    MarketListDto dto = new MarketListDto()
                    {
                        ParityName = item.ParityName,
                        Type = "Sell",
                        Price = item.Price,
                        Amount = item.Amount,
                        NetPrice = item.NetPrice,
                        Commission = item.Commission,
                        TotalPrice = item.TotalPrice,
                        Status = ((BaseStatus)item.Status).ToString(),
                        CreatedDate = item.CreatedDate,
                        ModifiedDate = item.ModifiedDate,
                    };
                    data.Add(dto);
                }
                foreach (var item in buyOrder)
                {
                    MarketListDto dto = new MarketListDto()
                    {
                        ParityName = item.ParityName,
                        Type = "Buy",
                        Price = item.Price,
                        Amount = item.Amount,
                        NetPrice = item.NetPrice,
                        Commission = item.Commission,
                        TotalPrice = item.TotalPrice,
                        Status = ((BaseStatus)item.Status).ToString(),
                        CreatedDate = item.CreatedDate,
                        ModifiedDate = item.ModifiedDate,

                    };
                    data.Add(dto);
                }

                return new SuccessDataResult<List<MarketListDto>>(data, Messages.List);
            }
            catch (Exception e)
            {

                return new ErrorDataResult<List<MarketListDto>>(Messages.ErrorList);
            }
        }

        public IDataResult<bool> OrderCancel(int buyOrderId, string token)
        {
            var buyorder = _buyOrderDal.Get(x => x.Id == buyOrderId);
            if (buyorder != null)
            {
                if (buyorder.Status == (int)BaseStatus.Waiting)
                {
                    var userToken = _tokenHelper.GetAuthenticatedUser(token);
                    var users = _userDal.Get(x => x.Id == userToken);

                    if (users == null)
                    {
                        return new ErrorDataResult<bool>(Messages.UserNotFound);
                    }
                    var parity = _parityDal.Get(x => x.Id == buyorder.ParityId);
                    var decreasingCoin = _walletDal.Get(x => x.UserId == users.Id && x.CryptoId == parity.Second); //azalan
                    if (decreasingCoin == null)
                    {
                        return new ErrorDataResult<bool>(false, Messages.Coin);
                    }

                    //kullanıcıya iade ediyoruz
                    decreasingCoin.Balance += buyorder.TotalPrice + buyorder.Commission;
                    _walletDal.Update(decreasingCoin);

                    //şirket cüzdanından komisyonu çıkartıyoruz.
                    var companyWallets = _companyWalletDal.Get(x => x.CryptoId == parity.Second);
                    companyWallets.Balance -= buyorder.Commission;
                    _companyWalletDal.Update(companyWallets);

                    buyorder.Status = (int)BaseStatus.Canceled;
                    _buyOrderDal.Update(buyorder);
                    return new SuccessDataResult<bool>(true, Messages.BuyOrderCancel);
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
        public IDataResult<List<BuyOrderListDto>> UserBuyOrderList(UserGetBuyOrderListFilterDto userGetBuyOrderListFilterDto)
        {
            try
            {
                if (userGetBuyOrderListFilterDto.PagingFilter.PageNumber <= 0)
                {
                    userGetBuyOrderListFilterDto.PagingFilter.PageNumber = 1;
                }
                if (userGetBuyOrderListFilterDto.PagingFilter.PageSize <= 0)
                {
                    userGetBuyOrderListFilterDto.PagingFilter.PageSize = 10;
                }
                var userToken = _tokenHelper.GetAuthenticatedUser(userGetBuyOrderListFilterDto.Token);
                var user = _userDal.Get(x => x.Id == userToken);
                if (user == null)
                {
                    return new ErrorDataResult<List<BuyOrderListDto>>(Messages.UserNotFound);
                }

                var totalCount = _buyOrderDal.GetList(x => x.UserId == userToken).Count;

                var buyOrders = _buyOrderDal.GetList().Where(x => x.UserId == user.Id).ToList();
                if (buyOrders.Count == 0)
                {
                    return new ErrorDataResult<List<BuyOrderListDto>>(Messages.UserOrdersNotFound);
                }
                var parities = _parityDal.GetList().ToList();

                var list = buyOrders
                            .Where(buyOrder => parities.Any(parity => parity.Id == buyOrder.ParityId))
                            .Select(buyOrder => new { buyOrder, parity = parities.FirstOrDefault(parity => parity.Id == buyOrder.ParityId) })
                            .Where(x => x.buyOrder.ParityName.Contains($"{userGetBuyOrderListFilterDto.Search}"))
                            .OrderByDescending(x => x.buyOrder.Price)
                            .Skip((userGetBuyOrderListFilterDto.PagingFilter.PageNumber - 1) * userGetBuyOrderListFilterDto.PagingFilter.PageSize)
                            .Take(userGetBuyOrderListFilterDto.PagingFilter.PageSize)
                            .ToList();

                if (list.Count == 0)
                {
                    return new ErrorDataResult<List<BuyOrderListDto>>(Messages.BuyOrderError);
                }

                var buyOrderList = list.Select(item => new BuyOrderListDto
                {
                    UserId = item.buyOrder.UserId,
                    SellerId = item.buyOrder.SellerId.Value,
                    SellerOrderId = item.buyOrder.SellerOrderId,
                    ParityName = item.buyOrder.ParityName,
                    Price = item.buyOrder.Price,
                    Amount = item.buyOrder.Amount,
                    NetPrice = item.buyOrder.NetPrice,
                    Commission = item.buyOrder.Commission,
                    TotalPrice = item.buyOrder.TotalPrice,
                    Status = ((BaseStatus)item.buyOrder.Status).ToString(),
                    CreatedDate = item.buyOrder.CreatedDate,
                    ModifiedDate = item.buyOrder.ModifiedDate

                })
                    .ToList();


                var pagination = new PagingDto
                {
                    PageNumber = userGetBuyOrderListFilterDto.PagingFilter.PageNumber,
                    PageSize = userGetBuyOrderListFilterDto.PagingFilter.PageSize,
                    TotalItemCount = totalCount,
                    TotalPageCount = (int)Math.Ceiling((double)totalCount / userGetBuyOrderListFilterDto.PagingFilter.PageSize),
                };

                return new SuccessDataResult<List<BuyOrderListDto>>(buyOrderList, Messages.List);
            }
            catch (Exception)
            {

                return new ErrorDataResult<List<BuyOrderListDto>>(Messages.ErrorList);
            }

        }
    }
}
