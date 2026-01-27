using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Helpers;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] RegisterDto dto)
        {
            try
            {
                _logger.LogInformation("Registering user: {Username}", dto.Username);
                await _authService.RegisterAsync(dto);
                return Ok(ApiResponse<string>.SuccessResponse(null, "User registered successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Internal server error"));
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<string>>> Login([FromBody] LoginDto dto)
        {
            try
            {
                _logger.LogInformation("User login attempt: {Username}", dto.Username);
                await _authService.LoginAsync(dto);
                return Ok(ApiResponse<string>.SuccessResponse(null, "User logged in successfully"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized login attempt for user: {Username}", dto.Username);
                return Unauthorized(ApiResponse<string>.ErrorResponse("Invalid username or password"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Internal server error"));
            }
        }
    }
}
