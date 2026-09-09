using System;

namespace Models.Fab
{
    public class DTOSalarySlip
    {
        public int? Slip_id { get; set; }
        public int? user_id { get; set; }
        public string? User_name { get; set; }
        public string? From_date { get; set; }
        public string? TO_date { get; set; }
        public int? Full_day { get; set; }
        public int? Half_day { get; set; }
        public int? Off_day { get; set; }
        public decimal? Full_salary { get; set; }
        public decimal? Half_salary { get; set; }
        public decimal? Advance_salary { get; set; }
        public decimal? Full_day_Total { get; set; }
        public decimal? Half_day_total { get; set; }
        public decimal? Advance_total { get; set; }
        public decimal? Grand_total { get; set; }
        public DateTime? Slip_day { get; set; }
    }
}
