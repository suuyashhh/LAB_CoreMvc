using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Farm
{
    public class DTOFarmEntry
    {
        public long FARM_ENTRY_ID { get; set; }
        public string ENTRY_TYPE { get; set; }
        public string REASON { get; set; }
        public long PRICE { get; set; }
        public long FARM_ID { get; set; }
        public long USER_ID { get; set; }
        public string IMAGE1 { get; set; }
        public string IMAGE2 { get; set; }
        public string IMAGE3 { get; set; }
        public string IMAGE4 { get; set; }
        public DateTime DATE { get; set; }
    }
}
