using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DairyFarm
{
    public class DTOBillDairy
    {
        public int? bill_id { get; set; }
        public int? user_id { get; set; }
        public string? animal_type { get; set; }
        public int? price { get; set; }
        public DateTime? date { get; set; }
        public string? BillImage { get; set; } // base64 image string
    }
}
