using TodoApi.DTOs;
using TodoApi.Entities;

namespace TodoApi.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItemDto>> GetAllAsync();
        Task<TaskItemDto?> GetByIdAsync(int id);
        Task<IEnumerable<TaskItemDto>> GetCompletedAsync();
        Task<TaskItemDto> CreateAsync(CreateTaskDto dto);
        Task<TaskItemDto?> CompleteTaskAsync(int id, CompleteTaskDto dto);
        Task<bool> DeleteAsync(int id);
        // Task<TaskItemDto?> MapToDto(TaskItem item);
    }
}
