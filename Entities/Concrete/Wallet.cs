using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Wallet : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public int CryptoId { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }

    }
}
