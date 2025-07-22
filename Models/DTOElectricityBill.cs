using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTOElectricityBill
    {
        public Int64 ELC_TRN_ID { get; set; }
        public Decimal ELC_PRICE { get; set; }
        public string DATE { get; set; }
        public string COM_ID { get; set; }
        public string CRT_BY { get; set; }
    }
}
