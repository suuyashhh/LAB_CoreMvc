using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Farm
{
    public class DTOLoginFarm
    {
        public long? USER_ID { get; set; }
        public string? USER_NAME { get; set; }
        public string? CONTACT { get; set; }
        public string? PASSWORD { get; set; }
        public DateTime? FROM_DATE { get; set; }
        public DateTime? TO_DATE { get; set; }
    }
}
