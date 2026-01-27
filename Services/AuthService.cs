using TodoApi.DTOs;
using TodoApi.Entities;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                Role = "User",
                Status = "Active",
            };

            await _authRepository.RegisterAsync(user);
        }

        public async Task LoginAsync(LoginDto dto)
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
        }

        private string HashPassword(string password)
        {
            // Implement a proper password hashing mechanism here
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}