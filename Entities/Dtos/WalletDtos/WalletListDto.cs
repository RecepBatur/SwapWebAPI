using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.WalletDtos
{
    public class WalletListDto : IDto
    {
        public decimal Balance { get; set; }
        public string CoinName { get; set; }

    }
}
