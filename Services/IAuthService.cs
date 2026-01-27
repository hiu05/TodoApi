using TodoApi.DTOs;
using TodoApi.Entities;

namespace TodoApi.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task LoginAsync(LoginDto dto);
    }
}