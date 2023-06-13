using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.CompanyWalletDtos
{
    public class CompanyWalletAddDto : IDto
    {
        public decimal Balance { get; set; }
        public int CryptoId { get; set; }
        public bool Status { get; set; }
    }
}
