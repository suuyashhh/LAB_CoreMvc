using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Tejas
{
    public class DTOTejasLogin
    {
        public int USER_ID { get; set; }
        public string? USER_NAME { get; set; }
        public string? PASS { get; set; }
        public string? CONTACT { get; set; }
        public string? USER_IMG { get; set; }
        public string? ROLE { get; set; }
    }
}
