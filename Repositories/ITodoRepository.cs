using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync(int userId);
        Task<TaskItem?> GetByIdAsync(int id);
        Task AddAsync(TaskItem item);
        Task CompleteTaskAsync(TaskItem item);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
