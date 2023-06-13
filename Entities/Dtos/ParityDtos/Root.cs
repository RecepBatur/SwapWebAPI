using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.ParityDtos
{
    public class Root
    {
        public string symbol { get; set; }
        public string price { get; set; }
    }
}
