namespace TodoApi.Helpers
{
    public static class PriorityHelper
    {
        public static string GetPriorityName(int priority)
        {
            return priority switch
            {
                0 => "Low",
                1 => "Medium",
                2 => "High",
                _ => "Unknown"
            };
        }

        public static int GetPriorityValue(string priorityName)
        {
            return priorityName.ToLower() switch
            {
                "low" => 0,
                "medium" => 1,
                "high" => 2,
                _ => 0
            };
        }
    }
}
