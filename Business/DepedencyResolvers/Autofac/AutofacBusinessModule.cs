using Autofac;
using Business.Abstract;
using Business.AdminAreas.Abstract;
using Business.AdminAreas.Concrete;
using Business.Concrete;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using DataAccess.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DepedencyResolvers.Autofac
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserManager>().As<IUserService>();
            builder.RegisterType<AdminUserManager>().As<IUserService>();
            builder.RegisterType<EfUserDal>().As<IUserDal>();

            builder.RegisterType<AuthManager>().As<IAuthService>();
            builder.RegisterType<AdminAuthManager>().As<IAdminAuthService>();
            builder.RegisterType<JwtHelper>().As<ITokenHelper>();

            builder.RegisterType<OperationClaimManager>().As<IOperationClaimService>();
            builder.RegisterType<AdminOperationClaimManager>().As<IOperationClaimService>();
            builder.RegisterType<EfOperationClaimDal>().As<IOperationClaimDal>();

            builder.RegisterType<UserOperationClaimManager>().As<IUserOperationClaimService>();
            builder.RegisterType<AdminUserOperationClaimManager>().As<IUserOperationClaimService>();
            builder.RegisterType<EfUserOperationClaimDal>().As<IUserOperationClaimDal>();

            builder.RegisterType<WalletManager>().As<IWalletService>();
            builder.RegisterType<AdminWalletManager>().As<IWalletService>();
            builder.RegisterType<EfWalletDal>().As<IWalletDal>();

            builder.RegisterType<AdminBuyOrderManager>().As<IBuyOrderService>();
            builder.RegisterType<BuyOrderManager>().As<IBuyOrderService>();
            builder.RegisterType<EfBuyOrderDal>().As<IBuyOrderDal>();

            builder.RegisterType<AdminSalesOrderManager>().As<ISalesOrderService>();
            builder.RegisterType<SalesOrderManager>().As<ISalesOrderService>();
            builder.RegisterType<EfSalesOrderDal>().As<ISalesOrderDal>();

            builder.RegisterType<ParityManager>().As<IParityService>();
            builder.RegisterType<AdminParityManager>().As<IParityService>();
            builder.RegisterType<EfParityDal>().As<IParityDal>();

            builder.RegisterType<AdminCoinManager>().As<IAdminCoinService>();
            builder.RegisterType<CoinManager>().As<ICoinService>();
            builder.RegisterType<EfCoinDal>().As<ICoinDal>();

            builder.RegisterType<CompanyWalletManager>().As<ICompanyWalletService>();
            builder.RegisterType<AdminCompanyWalletManager>().As<ICompanyWalletService>();
            builder.RegisterType<EfCompanyWalletDal>().As<ICompanyWalletDal>();
        }
    }
}
