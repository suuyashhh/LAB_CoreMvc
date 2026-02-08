using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DairyFarm
{
    public class DTODoctorDairy
    {
        public int? expense_id { get; set; }
        public int? user_id { get; set; }
        public int? Animal_id { get; set; }
        public string? expense_name { get; set; }
        public string? animal_name { get; set; }
        public string? reason { get; set; }
        public int? price { get; set; }
        public int? Switch { get; set; }
        public DateTime? date { get; set; }
        public string? AnimalImage { get; set; } // base64 image string
    }
}
