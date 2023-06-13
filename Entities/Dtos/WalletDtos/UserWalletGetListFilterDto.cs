using Core.Entities;
using Entities.Dtos.PagingDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.WalletDtos
{
    public class UserWalletGetListFilterDto : IDto
    {
        public string Search { get; set; }
        public PagingFilterDto PagingFilter { get; set; }
    }
}
