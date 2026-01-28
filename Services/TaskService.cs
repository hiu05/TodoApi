using TodoApi.DTOs;
using TodoApi.Entities;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TaskItemDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(MapToDto);
        }

        public async Task<TaskItemDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<IEnumerable<TaskItemDto>> GetCompletedAsync()
        {
            var items = await _repository.GetByCompletionStatusAsync();
            return items.Select(MapToDto);
        }

        public async Task<TaskItemDto> CreateAsync(CreateTaskDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required");

            var item = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UserId = dto.UserId
            };

            await _repository.AddAsync(item);
            return MapToDto(item);
        }

        public async Task<TaskItemDto?> CompleteTaskAsync(int id, CompleteTaskDto dto)
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

            await _repository.CompleteTaskAsync(item);
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

        private static TaskItemDto MapToDto(TaskItem item)
        {
            return new TaskItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                IsCompleted = item.IsCompleted,
                CreatedAt = item.CreatedAt,
                CompletedAt = item.CompletedAt,

            };
        }
    }
}
