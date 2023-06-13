using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.SaleOrderDtos
{
    public class BestSaleOrderListDto : IDto
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
