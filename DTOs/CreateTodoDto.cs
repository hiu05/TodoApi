namespace TodoApi.DTOs
{
    public class CreateTodoDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Priority { get; set; } = 0;
        public DateTime? DueDate { get; set; }
        public string? Category { get; set; }
    }
}
