using TodoApi.DTOs;
using TodoApi.Entities;

namespace TodoApi.Services
{
    public interface ITaskService
    {
        Task<PagedTaskItemDto> GetAllAsync(int pageNumber, int limit);
        Task<TaskItemDto?> GetByIdAsync(int id);
        Task<TaskItemDto> CreateAsync(CreateTaskDto dto);
        Task<TaskItemDto> UpdateAsync(int id, CreateTaskDto dto);
        Task CompleteTaskAsync(int id);
        Task<IEnumerable<TaskItemDto>> SearchAsync(string searchTerm);
        Task<PagedTaskItemDto> GetByTimeLineAsync(DateRangeDto dateRange);
        Task<bool> DeleteAsync(int id);
    }
}
