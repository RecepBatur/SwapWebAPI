using Business.Abstract;
using Business.AdminAreas.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Dtos.CoinDtos;
using Entities.Dtos.UserDtos;
using Entities.Dtos.WalletDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AdminAreas.Concrete
{
    public class AdminAuthManager : IAdminAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;
        private IOperationClaimDal _operationClaimDal;
        private IUserOperationClaimService _userOperationClaimService;
        private IUserOperationClaimDal _userOperationClaimDal;
        private IWalletService _walletService;
        private IUserDal _userDal;
        private ICoinDal _coinDal;
        public AdminAuthManager(IUserService userService, ITokenHelper tokenHelper, IUserOperationClaimService userOperationClaimService, IWalletService walletService, IUserDal userDal, IOperationClaimDal operationClaimDal, IUserOperationClaimDal userOperationClaimDal, ICoinDal coinDal)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _userOperationClaimService = userOperationClaimService;
            _walletService = walletService;
            _userDal = userDal;
            _operationClaimDal = operationClaimDal;
            _userOperationClaimDal = userOperationClaimDal;
            _coinDal = coinDal;
        }


        public IDataResult<bool> ChangePassword(UserChangePasswordDto userChangePasswordDto, string token)
        {
            var userToken = _tokenHelper.GetAuthenticatedUser(token);
            var users = _userDal.Get(x => x.Id == userToken);
            if (users == null)
            {
                return new ErrorDataResult<bool>(Messages.UserNotFound);
            }
            if (!HashingHelper.VerifyPasswordHash(userChangePasswordDto.CurrentPassword, users.PasswordHash, users.PasswordSalt))
            {
                return new ErrorDataResult<bool>(Messages.PasswordError);
            }
            if (userChangePasswordDto.NewPassword != userChangePasswordDto.NewPasswordAgain)
            {
                return new ErrorDataResult<bool>(Messages.WrongPassword);
            }

            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(userChangePasswordDto.NewPassword, out passwordHash, out passwordSalt);

            users.PasswordHash = passwordHash;
            users.PasswordSalt = passwordSalt;

            _userService.Update(users);


            return new SuccessDataResult<bool>(true, Messages.ChangePassword);
        }

        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            //kullanıcının kayıt ya da login olduktan sonra verilen token.
            var claims = _userService.GetClaims(user);
            var accessToken = _tokenHelper.CreateToken(user, claims);
            return new SuccessDataResult<AccessToken>(accessToken, Messages.AccessTokenCreated);
        }

        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            //kullanıcıdan bilgileri isteyip böyle bir kullanıcı var mı yok mu maili kontrol ettik.
            var userToCheck = _userService.GetByMail(userForLoginDto.Email);
            if (userToCheck == null)
            {
                return new ErrorDataResult<User>(Messages.UserNotFound);
            }
            //Kullanıcı giriş yaptıktan sonra hashli olan şifre db'deki şifre ile aynı mı onu kontrol edeceğiz.
            if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>(Messages.PasswordError);
            }
            return new SuccessDataResult<User>(userToCheck, Messages.SuccessDefaultLogin);
        }

        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User
            {
                Email = userForRegisterDto.Email,
                UserName = userForRegisterDto.UserName,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                Phone = userForRegisterDto.Phone,
                Address = userForRegisterDto.Address,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true,
            };

            _userService.Add(user);

            var admin = _operationClaimDal.Get(x => x.Name == "Admin");
            if (admin != null)
            {
                return new SuccessDataResult<User>(Messages.Success);
            }
            var userOperationClaim = new UserOperationClaim
            {
                UserId = user.Id,
                OperationClaimId = admin.Id,
                Status = true,
            };
            _userOperationClaimDal.Add(userOperationClaim);

            var coinList = _coinDal.GetList();
            if (coinList.Count == 0)
            {
                return new ErrorDataResult<User>(Messages.ErrorList);
            }
            foreach (var coinAdd in coinList)
            {
                var walletDto = new WalletAddDto
                {
                    UserId = user.Id,
                    Balance = 0,
                    CryptoId = coinAdd.Id,
                    Name = coinAdd.CoinName,
                    Status = true,
                };
                _walletService.Add(walletDto);
            }


            return new SuccessDataResult<User>(user, Messages.UserRegistered);
        }

        public IResult UserExists(string email)
        {
            if (_userService.GetByMail(email) != null)
            {
                return new ErrorResult(Messages.UserAlreadyExists);
            }
            return new SuccessResult();
        }

        public IResult UsernameControl(string username)
        {
            if (_userService.GetByUser(username) != null)
            {
                return new ErrorResult(Messages.UserControl);
            }
            return new SuccessResult();
        }
        public IResult UserPhoneControl(string phone)
        {
            if (_userService.GetByPhone(phone) != null)
            {
                return new ErrorResult(Messages.PhoneAlready);
            }

            return new SuccessResult();
        }
    }
}
