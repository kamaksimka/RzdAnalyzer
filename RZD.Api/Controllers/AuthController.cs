using Microsoft.AspNetCore.Mvc;
using RZD.Application.Models;
using RZD.Application.Services;
using System.Security.Claims;
using System.Text;


namespace RZD.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<TokensModel> Login([FromBody] LoginRequest request)
            => await _authService.LoginAsync(request);

        [HttpPost("register")]
        public async Task Register([FromBody] RegisterRequest request)
            => await _authService.RegisterAsync(request);

        [HttpPost("refresh")]
        public async Task<TokensModel> Refresh([FromBody] RefreshRequest request)
            => await _authService.RefreshAsync(request);

    }
}
