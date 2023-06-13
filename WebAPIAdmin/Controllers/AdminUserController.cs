using Business.Abstract;
using Entities.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace WebAPIAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private IUserService _userService;

        public AdminUserController(IUserService userService)
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
        [HttpPost(template: "usergetall")]
        public IActionResult UserGetAll(UserGetAllFilterDto userGetAllFilterDto)
        {
            var result = _userService.GetUserList(userGetAllFilterDto);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
