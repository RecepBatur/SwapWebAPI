using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos.ParityDtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AdminAreas.Concrete
{
    public class AdminParityManager : IParityService
    {
        private readonly IParityDal _parityDal;

        public AdminParityManager(IParityDal parityDal)
        {
            _parityDal = parityDal;
        }
        public IDataResult<bool> ParityAdd(ParityAddedDto parity)
        {
            var parityAdd = new Parity
            {
                Symbol = parity.Symbol,
                First = parity.First,
                Second = parity.Second,
                Commission = parity.Commission,
                Status = parity.Status,
            };
            _parityDal.Add(parityAdd);
            return new SuccessDataResult<bool>(true, Messages.Parity);
        }
        public async Task<IDataResult<List<Root>>> ParityGetList()
        {
            List<Root> parityDtoList = new List<Root>();
            using (var client = new HttpClient())
            {
                var responseEth = await client.GetAsync("https://api.binance.com/api/v3/ticker/price?symbol=ETHUSDT");
                var contentEth = await responseEth.Content.ReadAsStringAsync();
                var resultEth = JsonConvert.DeserializeObject<Root>(contentEth);
                parityDtoList.Add(resultEth);

                var responseBtc = await client.GetAsync("https://api.binance.com/api/v3/ticker/price?symbol=BTCUSDT");
                var contentBtc = await responseBtc.Content.ReadAsStringAsync();
                var resultBtc = JsonConvert.DeserializeObject<Root>(contentBtc);
                parityDtoList.Add(resultBtc);


            }
            return new SuccessDataResult<List<Root>>(parityDtoList, Messages.ParityList);
        }
    }
}
