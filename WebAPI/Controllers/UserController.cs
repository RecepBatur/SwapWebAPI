using Business.Abstract;
using Core.Entities.Concrete;
using Entities.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Member")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost(template: "updatestatus")]
        public IActionResult UpdateStatus(int userId)
        {
            var result = _userService.UpdateStatus(userId);
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPost(template: "updateuser")]
        public IActionResult UpdateUser(UserUpdateDto userUpdateDto, string token)
        {
            var result = _userService.UpdateUser(userUpdateDto, token);
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
