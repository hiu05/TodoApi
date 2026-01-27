namespace TodoApi.Entities
{
    public class User
    {
        public int Id { get; set; }   // Khóa chính
        public string Username { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }   
        public string Role { get; set; } = "user"; 
        public string Status { get; set; } = "offline"; // enum: online/offline/busy
        public DateTime? LastActiveAt { get; set; }
 
    }
}