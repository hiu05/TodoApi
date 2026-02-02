using TodoApi.DTOs;
using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public interface ITaskRepository
    {
        Task<PagedTaskItemDto> GetAllAsync(int userId, int pageNumber, int limit);
        Task<TaskItem?> GetByIdAsync(int id, int userId);
        Task<PagedTaskItemDto> GetByTimeLineAsync(DateRangeDto dateRange, int userId);
        Task AddAsync(TaskItem item);
        Task CompleteTaskAsync(int id, int userId);
        Task UpdateAsync(int id, int userId);
        Task DeleteAsync(int id, int userId);
        Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, int userId);
    }
}
