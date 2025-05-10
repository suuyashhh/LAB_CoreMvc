using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTOTestTable
    {
        public Int64 TEST_CODE { get; set; }
        public string TEST_NAME { get; set; }
        public Int64 TRN_NO { get; set; }
        public int SR_NO { get; set; }
        public decimal PRICE { get; set; }
        public decimal LAB_PRICE { get; set; }
        public string COM_ID { get; set; }
    }
}
