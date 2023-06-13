using Core.Utilities.Results;
using Entities.Dtos.CompanyWalletDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICompanyWalletService
    {
        IDataResult<bool> CompanyWalletAdd(CompanyWalletAddDto companyWalletAddDto);
    }
}
