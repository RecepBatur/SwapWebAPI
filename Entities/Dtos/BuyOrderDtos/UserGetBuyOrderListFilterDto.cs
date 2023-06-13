using Core.Entities;
using Entities.Dtos.PagingDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.BuyOrderDtos
{
    public class UserGetBuyOrderListFilterDto : IDto
    {
        public string Token { get; set; }
        public string Search { get; set; }
        public PagingFilterDto PagingFilter { get; set; }
    }
}
