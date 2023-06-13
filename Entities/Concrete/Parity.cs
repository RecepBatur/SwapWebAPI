using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Concrete
{
    public class Parity : IEntity
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        //public string Price { get; set; }
        public int First { get; set; }
        public int Second { get; set; }
        public int Commission { get; set; }
        public bool Status { get; set; }

    }
}
