namespace TodoApi.DTOs
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
    public class PaginationDto
    {
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
    }
    public class PagedTaskItemDto
    {
        public PaginationDto Pagination { get; set; } = new PaginationDto();
        public IEnumerable<TaskItemDto> Items { get; set; } = new List<TaskItemDto>();
    }
    public class PagedRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int Limit { get; set; } = 10;
    }
}
