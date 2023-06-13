using Entities.Dtos.PagingDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos.WalletDtos
{
    public class UserWalletGetListPagingDto
    {
        public List<UserWalletListDto> Data { get; set; }
        public PagingDto Page { get; set; }
    }
}
