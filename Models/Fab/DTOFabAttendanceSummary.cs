using System;

namespace Models.Fab
{
    public class DTOFabAttendanceSummary
    {
        public int? User_id { get; set; }
        public string? User_name { get; set; }
        public decimal? User_salary { get; set; }
        public decimal? TOTAL_ADVANCE { get; set; }
        public int? FullDay_Count { get; set; }
        public int? HalfDay_Count { get; set; }
        public int? OffDay_Count { get; set; }
    }
}
