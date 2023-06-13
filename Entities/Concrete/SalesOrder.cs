using Core.Entities;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class SalesOrder : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? BuyerId { get; set; }
        public int BuyerOrderId { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal NetPrice { get; set; }
        public string ParityName { get; set; }
        public int ParityId { get; set; }
        public decimal Commission { get; set; }
        public decimal TotalPrice { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
