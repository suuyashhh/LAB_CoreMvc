// AnimalHealthSummaryDTO.cs
namespace Models.DairyFarm
{
    public class AnimalHealthSummaryDTO
    {
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public int TotalHealthRecords { get; set; }
        public decimal TotalExpenses { get; set; }
        public string? AnimalImage { get; set; } 
    }
}

// AnimalHealthRecordDTO.cs
namespace Models.DairyFarm
{
    public class AnimalHealthRecordDTO
    {
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public string RecordType { get; set; } = string.Empty; // 'Medicine' or 'Doctor'
        public string Reason { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime RecordDate { get; set; }
        public string FormattedDate { get; set; } = string.Empty;
        public string MonthYear { get; set; } = string.Empty;
    }
}