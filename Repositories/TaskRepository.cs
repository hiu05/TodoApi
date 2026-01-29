using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TodoDbContext _context;

        public TaskRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<PagedTaskItemDto> GetAllAsync(int userId, int pageNumber, int limit)
        {
            var query = _context.Tasks
                 .Where(t => t.UserId == userId)
                 .OrderByDescending(t => t.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)limit);

            var items = await query
                .Skip((pageNumber - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return new PagedTaskItemDto
            {
                Pagination = new PaginationDto
                {
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    Limit = limit,
                    TotalPages = totalPages
                },
                Items = items.Select(item => new TaskItemDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    IsCompleted = item.IsCompleted,
                    CreatedAt = item.CreatedAt,
                    CompletedAt = item.CompletedAt
                })
            };
        }

        public async Task<TaskItem?> GetByIdAsync(int id, int userId)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task CompleteTaskAsync(int id, int userId)
        {
            var item = await GetByIdAsync(id, userId);
            if (item != null)
            {
                item.IsCompleted = true;
                item.CompletedAt = DateTime.UtcNow;
                _context.Tasks.Update(item);
                await SaveAsync();
            }
        }

        public async Task AddAsync(TaskItem item)
        {
            await _context.Tasks.AddAsync(item);
            await SaveAsync();
        }

        public async Task UpdateAsync(int id, int userId)
        {
            var item = await GetByIdAsync(id, userId);
            if (item != null)
            {
                _context.Tasks.Update(item);
                await SaveAsync();
            }
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var item = await GetByIdAsync(id, userId);
            if (item != null)
            {
                _context.Tasks.Remove(item);
                await SaveAsync();
            }
        }

        public async Task<IEnumerable<TaskItem>> SearchAsync(string searchTerm, int userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId && EF.Functions.Like(t.Title.ToLower().Trim(), $"%{searchTerm.ToLower().Trim()}%"))
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
