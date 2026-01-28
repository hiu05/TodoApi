using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public class TasksRepository : ITaskRepository
    {
        private readonly TodoDbContext _context;

        public TasksRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _context.Tasks.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<TaskItem>> GetByCompletionStatusAsync()
        {
            return await _context.Tasks
                .Where(x => x.IsCompleted == true)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
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
