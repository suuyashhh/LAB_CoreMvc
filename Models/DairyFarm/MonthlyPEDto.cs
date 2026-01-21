// MonthlyPEDto.cs
namespace Models.DairyFarm
{
    public class MonthlyPEDto
    {
        public string MonthName { get; set; }
        public int YearValue { get; set; }
        public int MonthNumber { get; set; }
        public decimal TotalBill { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Profit { get; set; }
    }

    public class MonthlyPEModel
    {
        public string DisplayMonth { get; set; }
        public decimal TotalBill { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Profit { get; set; }
        public string ProfitColor { get; set; }
    }
}