using TodoApi.DTOs;

namespace TodoApi.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidTitle(string? title)
        {
            return !string.IsNullOrWhiteSpace(title) && title.Length <= 255;
        }

        public static bool IsValidDescription(string? description)
        {
            return description == null || description.Length <= 1000;
        }

        public static bool IsValidCategory(string? category)
        {
            return category == null || category.Length <= 50;
        }

        public static bool IsValidPriority(int priority)
        {
            return priority >= 0 && priority <= 2;
        }

        public static bool IsValidDueDate(DateTime? dueDate)
        {
            return dueDate == null || dueDate > DateTime.UtcNow;
        }
    }
}
