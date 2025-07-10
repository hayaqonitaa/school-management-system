using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Modules.Auth.Dtos;
using SchoolManagementSystem.Modules.Auth.Services;
using SchoolManagementSystem.Common.Models;
using SchoolManagementSystem.Common.Helpers;

namespace SchoolManagementSystem.Modules.Auth
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
        public async Task<IActionResult> Login(LoginRequestDTO loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);
            return Ok(ApiResponseHelper.Success(response, "Login successful."));
        }
        
        // [HttpPost("register")]
        // public async Task<IActionResult> Register(RegisterRequestDTO registerRequest)
        // {
        //     var response = await _authService.RegisterAsync(registerRequest);
        //     return Ok(ApiResponseHelper.Success(response, "Registration successful."));
        // }
        
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin(LoginRequestDTO loginRequest)
        {
            var response = await _authService.LoginAdminAsync(loginRequest);
            return Ok(ApiResponseHelper.Success(response, "Admin login successful."));
        }
    }
}
