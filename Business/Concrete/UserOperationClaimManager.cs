using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserOperationClaimManager : IUserOperationClaimService
    {
        private readonly IUserOperationClaimDal _userOperationClaimDal;

        public UserOperationClaimManager(IUserOperationClaimDal userOperationClaimDal)
        {
            _userOperationClaimDal = userOperationClaimDal;
        }

        public IResult Add(UserOperationClaim userOperationClaim)
        {
            var addedRole = new UserOperationClaim
            {
                UserId = userOperationClaim.UserId,
                OperationClaimId = userOperationClaim.OperationClaimId,
                Status = true,
                CreatedDate = userOperationClaim.CreatedDate,
                ModifiedDate = userOperationClaim.ModifiedDate,
            };
            _userOperationClaimDal.Add(addedRole);
            return new SuccessResult(Messages.UserRole);
        }
    }
}
