using TodoApi.DTOs;
using TodoApi.Entities;

namespace TodoApi.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItemDto>> GetAllAsync();
        Task<TodoItemDto?> GetByIdAsync(int id);
        Task<IEnumerable<TodoItemDto>> GetCompletedAsync();
        Task<IEnumerable<TodoItemDto>> GetPendingAsync();
        Task<IEnumerable<TodoItemDto>> GetByCategoryAsync(string category);
        Task<TodoItemDto> CreateAsync(CreateTodoDto dto);
        Task<TodoItemDto?> UpdateAsync(int id, UpdateTodoDto dto);
        Task<bool> DeleteAsync(int id);
        Task<int> GetCompletionPercentageAsync();
    }
}
