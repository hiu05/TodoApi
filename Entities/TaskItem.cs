namespace TodoApi.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }                  // Khóa chính
        public string Title { get; set; } = string.Empty; // Tiêu đề công việc
        public string? Description { get; set; }     // Mô tả chi tiết
        public bool IsCompleted { get; set; } = false; // Trạng thái hoàn thành
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Thời điểm tạo
        public DateTime? CompletedAt { get; set; }   // Thời điểm hoàn thành (null nếu chưa xong)

        // Liên kết với User
        public int UserId { get; set; }
        public User User { get; set; }
    }
}