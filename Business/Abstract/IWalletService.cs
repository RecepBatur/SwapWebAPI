using Core.Utilities.Results;
using Entities.Dtos.WalletDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IWalletService
    {
        IDataResult<bool> Add(WalletAddDto wallet);
        IDataResult<List<WalletListDto>> UserWalletList(string token);
        IDataResult<UserWalletGetListPagingDto>UserWalletGetList(UserWalletGetListFilterDto userWalletGetListFilterDto);
    }
}
