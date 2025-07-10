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
            try
            {
                var response = await _authService.LoginAsync(loginRequest);
                return Ok(ApiResponseHelper.Success(response, "Login successful."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseHelper.Error<object>(ex.Message, StatusCodes.Status401Unauthorized));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Error<object>(ex.Message, StatusCodes.Status400BadRequest));
            }
        }
        
        // [HttpPost("register")]
        // public async Task<IActionResult> Register(RegisterRequestDTO registerRequest)
        // {
        //     try
        //     {
        //         var response = await _authService.RegisterAsync(registerRequest);
        //         return Ok(response);
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { message = ex.Message });
        //     }
        // }
        
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin(LoginRequestDTO loginRequest)
        {
            try
            {
                var response = await _authService.LoginAdminAsync(loginRequest);
                return Ok(ApiResponseHelper.Success(response, "Admin login successful."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseHelper.Error<object>(ex.Message, StatusCodes.Status401Unauthorized));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelper.Error<object>(ex.Message, StatusCodes.Status400BadRequest));
            }
        }
    }
}
