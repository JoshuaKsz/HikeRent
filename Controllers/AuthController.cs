using HikeRent.DTOs.Auth;
using HikeRent.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HikeRent.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login (
            [FromBody] LoginRequest request)
        {
            LoginResponse? result = await _authService.LoginAsync(request);

            if(result == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    messsage = "Email atau password salah."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Login berhasil.",
                data = result
            });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            return Ok(new
            {
                success = true,
                message = "Token valid.",
                userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                fullName = User.Identity?.Name,
                email = User.FindFirst(ClaimTypes.Email)?.Value,
                role = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }
    }
}
