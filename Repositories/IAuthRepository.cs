using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public interface IAuthRepository
    {
        Task RegisterAsync(User user);
        Task<User?> LoginAsync(User user);
    }
}
