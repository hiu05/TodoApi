using Microsoft.AspNetCore.Http.HttpResults;
using TodoApi.DTOs;
using TodoApi.Entities;
using TodoApi.Helpers;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository repository, IHttpContextAccessor httpContextAccessor, ILogger<TaskService> logger)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        private int _userId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(claim))
                    throw new UnauthorizedAccessException("UserId not found in token");
                return int.Parse(claim);
            }
        }

        public async Task<IEnumerable<TaskItemDto>> GetAllAsync()
        {
            // var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            // if (string.IsNullOrEmpty(userIdClaim))
            //     throw new UnauthorizedAccessException("UserId not found in token");

            // var userId = int.Parse(userIdClaim);
            _logger.LogInformation("Fetching tasks for UserId: {UserId}", _userId);
            var items = await _repository.GetAllAsync(_userId);
            return items.Select(MapToDto);
        }

        public async Task<TaskItemDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<TaskItemDto> CreateAsync(CreateTaskDto dto)
        {
            var item = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow,
                UserId = _userId
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
