using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.SmartParking
{
    public class DTOParkingRegistration
    {
        public int? USERID { get; set; }
        public string? NAME { get; set; }
        public string? EMAIL { get; set; }
        public string? PASS { get; set; }
        public string? PHONE { get; set; }
    }
}
