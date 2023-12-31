﻿using Business.Abstract;
using Entities.Dtos.UserDtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost(template: "login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            //kullanıcıyı login ederek kontrol edeceğiz.
            var userToLogin = _authService.Login(userForLoginDto);
            if (!userToLogin.Success) //kullanıcı girişi başarılı değilse
            {
                return BadRequest(userToLogin.Message);
            }
            //login işlemi başarılı ise kullanıcıya token üretmesi.
            var result = _authService.CreateAccessToken(userToLogin.Data);
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
            var userExists = _authService.UserExists(userForRegisterDto.Email);
            if (!userExists.Success)//eğer kulllanıcı varsa
            {
                return BadRequest(userExists.Message);
            }
            var usernameControl = _authService.UsernameControl(userForRegisterDto.UserName);
            if (!usernameControl.Success)
            {
                return BadRequest(usernameControl.Message);
            }
            var phoneToCheck = _authService.UserPhoneControl(userForRegisterDto.Phone);
            if (!phoneToCheck.Success)
            {
                return BadRequest(phoneToCheck.Message);
            }

            //eğer kullanıcı yoksa
            var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password);
            //var result = _authService.CreateAccessToken(registerResult.Data);
            if (registerResult.Success)
            {
                //kullanıcıya burada bir user döndürüp token veriyoruz.
                return Ok(registerResult.Success);
            }
            return BadRequest(registerResult.Message);
        }
        [HttpPost(template: "changepassword")]
        [Authorize(Roles = "Member")]
        public IActionResult ChangePassword(UserChangePasswordDto userChangePasswordDto, string token)
        {
            var result = _authService.ChangePassword(userChangePasswordDto, token);
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
