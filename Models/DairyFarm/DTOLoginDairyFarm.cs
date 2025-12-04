using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DairyFarm
{
    public class DTOLoginDairyFarm
    {
        public int? user_id {  get; set; }
        public string? user_name {  get; set; }
        public string? password { get; set; }
        public string? contact { get; set; }
    }
}
