using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Helpers;
using TodoApi.Services;
using TodoApi.Validators;

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
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterDto dto)
        {
            try
            {
                AuthValidator.ValidateRegisterDto(dto);
                _logger.LogInformation("Registering user: {Username}", dto.Username);
                await _authService.RegisterAsync(dto);
                return Ok(ApiResponse.SuccessResponse("User registered successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error during registration");
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto dto)
        {
            try
            {   
                AuthValidator.ValidateLoginDto(dto);
                _logger.LogInformation("User login attempt: {Username}", dto.Username);
                var response = await _authService.LoginAsync(dto);
                return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "User logged in successfully"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Unauthorized login attempt for user: {Username}", dto.Username);
                return Unauthorized(ApiResponse.ErrorResponse("Invalid username or password"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, ApiResponse.ErrorResponse("Internal server error"));
            }
        }
    }
}
