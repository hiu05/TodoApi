using TodoApi.DTOs;
using TodoApi.Entities;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly ITodoRepository _repository;

        public TodoService(ITodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TodoItemDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(MapToDto);
        }

        public async Task<TodoItemDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<IEnumerable<TodoItemDto>> GetCompletedAsync()
        {
            var items = await _repository.GetByCompletionStatusAsync(true);
            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<TodoItemDto>> GetPendingAsync()
        {
            var items = await _repository.GetByCompletionStatusAsync(false);
            return items.Select(MapToDto);
        }

        public async Task<IEnumerable<TodoItemDto>> GetByCategoryAsync(string category)
        {
            var items = await _repository.GetByCategoryAsync(category);
            return items.Select(MapToDto);
        }

        public async Task<TodoItemDto> CreateAsync(CreateTodoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required");

            var item = new TodoItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                Category = dto.Category?.Trim(),
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(item);
            return MapToDto(item);
        }

        public async Task<TodoItemDto?> UpdateAsync(int id, UpdateTodoDto dto)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return null;

            if (!string.IsNullOrWhiteSpace(dto.Title))
                item.Title = dto.Title.Trim();

            if (dto.Description != null)
                item.Description = dto.Description.Trim();

            if (dto.IsCompleted.HasValue)
                item.IsCompleted = dto.IsCompleted.Value;

            if (dto.Priority.HasValue)
                item.Priority = dto.Priority.Value;

            if (dto.DueDate != null)
                item.DueDate = dto.DueDate;

            if (dto.Category != null)
                item.Category = dto.Category.Trim();

            await _repository.UpdateAsync(item);
            return MapToDto(item);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                return false;

            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<int> GetCompletionPercentageAsync()
        {
            var items = await _repository.GetAllAsync();
            var totalItems = items.Count();
            if (totalItems == 0)
                return 0;

            var completedItems = items.Count(x => x.IsCompleted);
            return (completedItems * 100) / totalItems;
        }

        private static TodoItemDto MapToDto(TodoItem item)
        {
            return new TodoItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted,
                Priority = item.Priority,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                DueDate = item.DueDate,
                Category = item.Category
            };
        }
    }
}
