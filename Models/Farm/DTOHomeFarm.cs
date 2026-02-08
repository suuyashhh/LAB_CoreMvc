using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Farm
{
    public class DTOHomeFarm
    {
        public int FARM_ID { get; set; }
        public string FARM_NAME { get; set; }
        public string USER_ID { get; set; }
        public string IMAGE { get; set; }  // base64
    }
}
