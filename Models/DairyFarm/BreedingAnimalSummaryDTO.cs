// BreedingAnimalSummaryDTO.cs
namespace Models.DairyFarm
{
    public class BreedingAnimalSummaryDTO
    {
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public int TotalBreedingRecords { get; set; }
        public DateTime? LastBreedingDate { get; set; }
        public string LastBreedingReason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}

// BreedingRecordDTO.cs
namespace Models.DairyFarm
{
    public class BreedingRecordDTO
    {
        public int RecordId { get; set; }
        public int UserId { get; set; }
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime BreedingDate { get; set; }
        public string FormattedDate { get; set; } = string.Empty;
        public string MonthYear { get; set; } = string.Empty;
        public int DaysSinceBreeding { get; set; }
        public string Status { get; set; } = string.Empty; // "Recent", "Ongoing", "Overdue"
    }
}