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

namespace Business.AdminAreas.Concrete
{
    public class AdminCompanyWalletManager : ICompanyWalletService
    {
        private readonly ICompanyWalletDal _companyWalletDal;

        public AdminCompanyWalletManager(ICompanyWalletDal companyWalletDal)
        {
            _companyWalletDal = companyWalletDal;
        }

        public IDataResult<bool> CompanyWalletAdd(CompanyWalletAddDto companyWalletAddDto)
        {
            var companyWallet = new CompanyWallet
            {
                Balance = 0,
                CryptoId = companyWalletAddDto.CryptoId,
                Status = true,
            };
            _companyWalletDal.Add(companyWallet);

            return new SuccessDataResult<bool>(true, Messages.WalletAdded);
        }
    }
}
