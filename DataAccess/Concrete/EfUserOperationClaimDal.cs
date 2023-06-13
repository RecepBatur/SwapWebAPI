using Core.DataAccess.EfEntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class EfUserOperationClaimDal : EfEntityRepositoryBase<UserOperationClaim, SwapContext>, IUserOperationClaimDal
    {
    }
}
