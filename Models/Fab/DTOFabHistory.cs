using System;

namespace Models.Fab
{
    public class DTOFabHistory
    {
        public int Record_id { get; set; }
        public int? User_id { get; set; }
        public string? Record_type { get; set; }
        public string? Record_name { get; set; }
        public decimal Price { get; set; }
        public DateTime Record_date { get; set; }
    }
}
