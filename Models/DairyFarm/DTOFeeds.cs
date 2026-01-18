using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DairyFarm
{
    public class DTOFeeds
    {
        public int? expense_id { get; set; }
        public int? user_id { get; set; }
        public int? feed_id { get; set; }
        public string? expense_name { get; set; }
        public string? feed_name { get; set; }
        public int? price { get; set; }    
        public string? quantity { get; set; }
        public DateTime? date {  get; set; }
        public string? FeedImage { get; set; } // base64 image string
    }
}
