// DatePEDto.cs
namespace Models.DairyFarm
{
    public class DatePEDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalBill { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Profit { get; set; }
    }

    public class DatePEQueryDto
    {
        public int UserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class DatePEModel
    {
        public string DisplayDateRange { get; set; }
        public decimal TotalBill { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Profit { get; set; }
        public string ProfitColor { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}