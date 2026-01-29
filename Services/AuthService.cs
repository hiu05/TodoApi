using TodoApi.DTOs;
using TodoApi.Entities;
using TodoApi.Repositories;
using TodoApi.Helpers;

namespace TodoApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public AuthService(IAuthRepository authRepository, JwtTokenHelper jwtTokenHelper)
        {
            _authRepository = authRepository;
            _jwtTokenHelper = jwtTokenHelper;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                Email = dto.Email,
                Role = "User",
                Status = "Active",
            };

            await _authRepository.RegisterAsync(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password)
            };

            var loggedInUser = await _authRepository.LoginAsync(user);
            if (loggedInUser == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Generate JWT token
            var token = _jwtTokenHelper.GenerateToken(loggedInUser);
            
            return new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(60)
            };
        }

        private string HashPassword(string password)
        {
            // Implement a proper password hashing mechanism here
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}