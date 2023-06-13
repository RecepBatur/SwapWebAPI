using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Dtos.UserDtos;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly ITokenHelper _tokenHelper;

        public UserManager(IUserDal userDal, ITokenHelper tokenHelper)
        {
            _userDal = userDal;
            _tokenHelper = tokenHelper;
        }

        public IResult Add(User user)
        {
            UserValidator userValidator = new UserValidator();
            var result = userValidator.Validate(user);
            //if (!result.IsValid)
            //{
            //    throw new ValidationException(result.Errors);
            //}

            if (!result.IsValid)
            {
                var errorBuilder = new StringBuilder();
                foreach (var error in result.Errors)
                {
                    errorBuilder.AppendLine(error.ErrorMessage);
                }
                throw new ValidationException(errorBuilder.ToString().Trim());
            }
            _userDal.Add(user);

            return new SuccessResult(Messages.UserAdded);
        }

        public User GetByMail(string email)
        {
            return _userDal.Get(filter: u => u.Email == email);
        }

        public List<OperationClaim> GetClaims(User user)
        {
            return _userDal.GetClaims(user);
        }

        public IDataResult<bool> UpdateStatus(int userId)
        {
            var user = _userDal.Get(x => x.Id == userId);
            if (user != null)
            {
                user.Status = !user.Status;
                _userDal.Update(user);
                return new SuccessDataResult<bool>(true, Messages.UserStatus);
            }
            else
            {

                return new ErrorDataResult<bool>(false, Messages.UserNotFound);
            }
        }

        public User GetByUser(string username)
        {
            return _userDal.Get(filter: u => u.UserName == username);
        }

        public IDataResult<User> GetById(int userId)
        {
            return (IDataResult<User>)_userDal.Get(x => x.Id == userId);

        }

        public IDataResult<bool> Update(User user)
        {
            _userDal.Update(user);

            return new SuccessDataResult<bool>(true);

        }

        public IDataResult<bool> UpdateUser(UserUpdateDto userUpdateDto, string token)
        {
            var userToken = _tokenHelper.GetAuthenticatedUser(token);
            var user = _userDal.Get(x => x.Id == userToken);
            if (user == null)
            {
                return new ErrorDataResult<bool>(Messages.UserNotFound);
            }

            user.UserName = userUpdateDto.UserName;
            user.Email = userUpdateDto.Email;
            user.Address = userUpdateDto.Address;
            user.Phone = userUpdateDto.Phone;

            _userDal.Update(user);

            return new SuccessDataResult<bool>(true, Messages.UserUpdate);
        }

        public User GetByPhone(string phone)
        {
            return _userDal.Get(filter: u => u.Phone == phone);
        }

        public IDataResult<UserGetAllPagingDto> GetUserList(UserGetAllFilterDto userGetAllFilterDto)
        {
            throw new NotImplementedException();
        }
    }
}