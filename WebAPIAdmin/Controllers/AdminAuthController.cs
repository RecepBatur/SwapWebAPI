using Business.Abstract;
using Business.AdminAreas.Abstract;
using Entities.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private IAdminAuthService _adminAuthService;

        public AdminAuthController(IAdminAuthService adminAuthService)
        {
            _adminAuthService = adminAuthService;
        }
        [HttpPost(template: "login")]
        //[Authorize(Roles = "Admin")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            //kullanıcıyı login ederek kontrol edeceğiz.
            var userToLogin = _adminAuthService.Login(userForLoginDto);
            if (!userToLogin.Success) //kullanıcı girişi başarılı değilse
            {
                return BadRequest(userToLogin.Message);
            }
            //login işlemi başarılı ise kullanıcıya token üretmesi.
            var result = _adminAuthService.CreateAccessToken(userToLogin.Data);
            if (result.Success) //başarılı ise tokeni veriyoruz.
            {
                return Ok(result.Data);
            }
            //başarısız olma durumunda
            return BadRequest(result.Message);
        }
        [HttpPost(template: "register")]
        public ActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            //kayıt olurken böyle bir kullanıcı var mı kontrol ediyoruz.
            var userExists = _adminAuthService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success)//eğer kulllanıcı varsa
            {
                return BadRequest(userExists.Message);
            }
            var usernameControl = _adminAuthService.UsernameControl(userForRegisterDto.UserName);
            if (!usernameControl.Success)
            {
                return BadRequest(usernameControl.Message);
            }
            var phoneToCheck = _adminAuthService.UserPhoneControl(userForRegisterDto.Phone);
            if (!phoneToCheck.Success)
            {
                return BadRequest(phoneToCheck.Message);
            }
            //eğer kullanıcı yoksa
            var registerResult = _adminAuthService.Register(userForRegisterDto, userForRegisterDto.Password);
            //var result = _adminAuthService.CreateAccessToken(registerResult.Data);
            if (registerResult.Success)
            {
                //kullanıcıya burada bir user döndürüp token veriyoruz.
                return Ok(registerResult.Success);
            }
            return BadRequest(registerResult.Message);
        }
        [HttpPost(template: "changepassword")]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangePassword(UserChangePasswordDto userChangePasswordDto, string token)
        {
            var result = _adminAuthService.ChangePassword(userChangePasswordDto, token);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
