using System;

namespace Models.Fab
{
    public class DTOFabMonthlyPE
    {
        public string? MonthName { get; set; }
        public int? YearValue { get; set; }
        public decimal? TotalBill { get; set; }
        public decimal? TotalExpense { get; set; }
        public decimal? Profit { get; set; }
    }
}
