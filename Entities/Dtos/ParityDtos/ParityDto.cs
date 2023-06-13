using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.ParityDtos
{
    public class ParityDto : IDto
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
    }
}
