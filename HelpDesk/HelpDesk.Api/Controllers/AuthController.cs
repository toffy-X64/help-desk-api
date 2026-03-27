using HelpDesk.Application.Services.Auth;
using HelpDesk.Application.Services.Users;
using HelpDesk.Shared.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDesk.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Result<UserDto> loginResult = await _authService.Login(request);
            if (!loginResult.IsSuccess)
                return Unauthorized(new { error = loginResult.Error?.Description });

            return Ok(loginResult.Value);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            Result<UserDto> registerResult = await _authService.Register(request);
            if (!registerResult.IsSuccess)
                return Unauthorized(new { error = registerResult.Error?.Description });

            return Ok(registerResult.Value);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            Guid userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            Result<UserDto> meResult = await _authService.Me(userId);

            if (!meResult.IsSuccess)
                return Unauthorized(new { error = meResult.Error?.Description });

            return Ok(meResult.Value);
        }
    }
}
