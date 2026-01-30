using TodoApi.DTOs;

namespace TodoApi.Validators
{
    public static class AuthValidator
    {
        public static void ValidateRegisterDto(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length > 50 || dto.Username.Length < 3)
            {
                throw new ArgumentException("Username is required and must be between 3 and 50 characters.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            {
                throw new ArgumentException("Password is required and must be at least 6 characters long.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email) || !dto.Email.Contains("@"))
            {
                throw new ArgumentException("A valid email address is required.");
            }
        }
        public static void ValidateLoginDto(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                throw new ArgumentException("Username is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException("Password is required.");
            }
        }
        
    }
}