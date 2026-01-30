using TodoApi.DTOs;
using TodoApi.Entities;
using TodoApi.Repositories;
using TodoApi.Helpers;
using Microsoft.AspNetCore.Identity;

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
                PasswordHash = dto.Password
            };

            var loggedInUser = await _authRepository.LoginAsync(user);
            if (loggedInUser == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
            if (!VerifyPassword(loggedInUser.PasswordHash, dto.Password))
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
            // Generate JWT token
            var token = _jwtTokenHelper.GenerateToken(loggedInUser);

            return new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(36)
            };
        }

        private string HashPassword(string password)
        {
            var hasher = new PasswordHasher<object>();
            return hasher.HashPassword(new object(), password);
        }

        private bool VerifyPassword(string hashedPassword, string password)
        {
            var hasher = new PasswordHasher<object>();
            var result = hasher.VerifyHashedPassword(new object(), hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }

    }
}