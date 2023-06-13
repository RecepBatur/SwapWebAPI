using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Dtos.UserDtos;
using Snickler.EFCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Business.AdminAreas.Concrete
{
    public class AdminUserManager : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly ITokenHelper _tokenHelper;

        public AdminUserManager(IUserDal userDal, ITokenHelper tokenHelper)
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
                throw new ValidationException(errorBuilder.ToString());
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

        //public IDataResult<List<UserListDto>> GetList()
        //{
        //    var list = _userDal.GetList();
        //    var userlist = new List<UserListDto>();
        //    foreach (var user in list)
        //    {
        //        userlist.Add(new()
        //        {
        //            Id = user.Id,
        //            Status = user.Status,
        //            CreatedDate = user.CreatedDate,
        //            ModifiedDate = user.ModifiedDate,
        //        });
        //    }
        //    return new SuccessDataResult<List<UserListDto>>(userlist, Messages.PlaylistList);
        //}

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

        //public IDataResult<bool> UpdatePassword(UserPasswordDto userUpdateDto)
        //{
        //    var updatedUser = new User
        //    {
        //        Id = userUpdateDto.Id,
        //        PasswordHash = passwordHash,
        //        PasswordSalt = passwordSalt
        //    };
        //    _userDal.Update(updatedUser);

        //    return new SuccessDataResult<bool>(true, Messages.ChangePassword);
        //}

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
            return _userDal.Get(filter: x => x.Phone == phone);
        }

        public IDataResult<UserGetAllPagingDto> GetUserList(UserGetAllFilterDto userGetAllFilterDto)
        {
            try
            {
                var pagingFilter = userGetAllFilterDto.PagingFilter;
                pagingFilter.PageNumber = pagingFilter.PageNumber <= 1 ? 0 : pagingFilter.PageNumber - 1;
                pagingFilter.PageSize = pagingFilter.PageSize <= 1 ? 1 : pagingFilter.PageSize;

                var skip = pagingFilter.PageSize * pagingFilter.PageNumber;
                var totalCount = 0;
                var dto = new List<UserGetAllDto>();

                using (var dbContext = new SwapContext())
                {
                    dbContext.LoadStoredProc("dbo.UserList")
                        .ExecuteStoredProc((handler) =>
                        {
                            var list = handler.ReadToList<UserGetAllDto>().ToList();
                            totalCount = list.Count;
                            dto = list.Skip(skip).Take(pagingFilter.PageSize).ToList();

                        });
                }

                var result = new UserGetAllPagingDto
                {
                    Data = dto,
                    Paging = new Entities.Dtos.PagingDtos.PagingDto
                    {
                        PageNumber = pagingFilter.PageNumber + 1,
                        PageSize = pagingFilter.PageSize,
                        TotalItemCount = totalCount,
                        TotalPageCount = (int)Math.Ceiling((double)totalCount / pagingFilter.PageSize)
                    }

                };

                return new SuccessDataResult<UserGetAllPagingDto>(result, Messages.List);
            }
            catch (Exception)
            {

                return new ErrorDataResult<UserGetAllPagingDto>(Messages.UnknownError);
            }


        }
    }
}
