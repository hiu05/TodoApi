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

        public async Task<IEnumerable<TaskItem>> GetAllAsync(int userId)
        {
            return await _context.Tasks
            .Where(u => u.UserId == userId)
            .Select(x => new TaskItem
            {
                Id = x.Id,
                Title = x.Title,
                IsCompleted = x.IsCompleted,
                CreatedAt = x.CreatedAt,
                CompletedAt = x.CompletedAt,
                UserId = x.UserId
            })
            .OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task AddAsync(TaskItem item)
        {
            await _context.Tasks.AddAsync(item);
            await SaveAsync();
        }

        public async Task CompleteTaskAsync(TaskItem item)
        {
            item.IsCompleted = true;
            item.CompletedAt = DateTime.UtcNow;
            _context.Tasks.Update(item);
            await SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if (item != null)
            {
                _context.Tasks.Remove(item);
                await SaveAsync();
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
