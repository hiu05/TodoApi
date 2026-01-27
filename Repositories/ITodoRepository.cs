using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public interface ITodoRepository
    {
        Task<IEnumerable<TodoItem>> GetAllAsync();
        Task<TodoItem?> GetByIdAsync(int id);
        Task<IEnumerable<TodoItem>> GetByCompletionStatusAsync(bool isCompleted);
        Task<IEnumerable<TodoItem>> GetByCategoryAsync(string category);
        Task AddAsync(TodoItem item);
        Task UpdateAsync(TodoItem item);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
