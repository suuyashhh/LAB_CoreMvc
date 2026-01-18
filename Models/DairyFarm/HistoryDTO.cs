namespace Models.DairyFarm
{
    public class HistoryDTO
    {
        // Common fields
        public long expense_id { get; set; }
        public int user_id { get; set; }
        public string expense_name { get; set; } = string.Empty;
        public decimal price { get; set; }
        public DateTime date { get; set; }
        public string? image { get; set; }

        // Conditional fields based on expense_name
        public string? feed_name { get; set; }
        public string? quantity { get; set; }
        public string? reason { get; set; }
        public string? animal_name { get; set; }
        public string? animal_type { get; set; }

        // Additional info for display
        public string DisplayTitle { get; set; } = string.Empty;
        public string DisplaySubtitle { get; set; } = string.Empty;
        public string IconClass { get; set; } = string.Empty;
        public string BadgeColor { get; set; } = string.Empty;
    }
}