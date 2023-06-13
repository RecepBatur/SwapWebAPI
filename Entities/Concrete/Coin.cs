using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Coin : IEntity
    {
        public int Id { get; set; }
        public string CoinName { get; set; }
        public bool Status { get; set; }
    }
}
