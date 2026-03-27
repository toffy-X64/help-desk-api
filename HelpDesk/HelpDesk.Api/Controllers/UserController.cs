using Azure.Core;
using HelpDesk.Application.Services.Auth;
using HelpDesk.Application.Services.Users;
using HelpDesk.Application.Services.Users.Requests;
using HelpDesk.Core.Abstractions;
using HelpDesk.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers
{
    [Authorize(Policy = "admin-only")]
    [ApiController]
    [Route("/api/v1/users")]
    public class UserController : ControllerBase
    {
        private readonly UsersService _usersService;

        public UserController(UsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersRequest query)
        {
            Result<IEnumerable<UserDto>> usersResult = await _usersService.GetAllUsers(query);
            if (!usersResult.IsSuccess)
            {
                return BadRequest(usersResult.Error.Description);
            }

            return Ok(usersResult.Value);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            Result<UserDto> deleteResult = await _usersService.DeleteUser(id);
            if (!deleteResult.IsSuccess)
            {
                return BadRequest(deleteResult.Error.Description);
            }

            return Ok(deleteResult.Value);
        }
    }
}
