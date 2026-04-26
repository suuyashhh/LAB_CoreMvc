using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTOParkingProvider
    {
        public int? SpotId { get; set; }
        public int? UserId { get; set; }

        public string? VehicalType { get; set; }
        public string? LatitudeLangitude { get; set; }
        public string? FullAddress { get; set; }
        public string? price { get; set; }
        public string? contact { get; set; }
        public string? img1 { get; set; }
        public string? img2 { get; set; }
        public string? img3 { get; set; }
        public string? img4 { get; set; }
    }
}
