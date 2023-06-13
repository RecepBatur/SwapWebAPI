using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Entities.Dtos.UserDtos;
using Entities.Dtos.WalletDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password);
        IDataResult<User> Login(UserForLoginDto userForLoginDto);
        IResult UserExists(string email); // Daha önce böyle bir email kayıtlı mı?
        IResult UsernameControl(string username);
        IResult UserPhoneControl(string phone);
        IDataResult<AccessToken> CreateAccessToken(User user);
        IDataResult<bool> ChangePassword(UserChangePasswordDto userChangePasswordDto, string token);
    }
}