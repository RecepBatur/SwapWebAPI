﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.BuyOrderDtos
{
    public class BuyOrderListDto : IDto
    {
        public int UserId { get; set; }
        public int SellerId { get; set; }
        public int SellerOrderId { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        //public int ParityId { get; set; }
        public string ParityName { get; set; }
        public decimal Commission { get; set; }
        public decimal NetPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
