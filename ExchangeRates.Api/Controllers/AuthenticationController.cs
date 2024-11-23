using System.Security.Claims;
using ExchangeRates.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRates.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public AuthenticationController(JwtTokenService tokenService)
        {
            _tokenService = tokenService;
        }

        private readonly JwtTokenService _tokenService;

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest login)
        {
            if (login.Username == "user" && login.Password == "password")
            {
                var token = _tokenService.GenerateToken(login.Username, "Admin");
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid credentials");
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
