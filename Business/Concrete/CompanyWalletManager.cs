using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos.CompanyWalletDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CompanyWalletManager : ICompanyWalletService
    {
        private readonly ICompanyWalletDal _companyWalletDal;

        public CompanyWalletManager(ICompanyWalletDal companyWalletDal)
        {
            _companyWalletDal = companyWalletDal;
        }

        public IDataResult<bool> CompanyWalletAdd(CompanyWalletAddDto companyWalletAddDto)
        {
            var companyWallet = new CompanyWallet
            {
                Balance = companyWalletAddDto.Balance,
                CryptoId = companyWalletAddDto.CryptoId,
                Status = true,
            };
            _companyWalletDal.Add(companyWallet);

            return new SuccessDataResult<bool>(true, Messages.WalletAdded);
        }
    }
}
