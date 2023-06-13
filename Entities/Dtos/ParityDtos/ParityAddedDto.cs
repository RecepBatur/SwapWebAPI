using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.ParityDtos
{
    public class ParityAddedDto : IDto
    {
        public string Symbol { get; set; }
        public int First { get; set; }
        public int Second { get; set; }
        public int Commission { get; set; }
        public bool Status { get; set; }

    }
}
