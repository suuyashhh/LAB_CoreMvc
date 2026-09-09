using System;

namespace Models.Fab
{
    public class DTOFabExpanse
    {
        public int? Exp_id { get; set; }
        public int? User_id { get; set; }
        public string? Exp_name { get; set; }
        public decimal? Exp_price { get; set; }
        public decimal? User_advance { get; set; }
        public DateTime? date { get; set; }
        
        // Joined columns
        public string? User_name { get; set; }
    }
}
