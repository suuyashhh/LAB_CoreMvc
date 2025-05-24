using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTOOtherExpense
    {
        public Int64 OTHER_ID { get; set; }
        public string OTHER_NAME { get; set; }
        public decimal OTHER_PRICE { get; set; }
        public string DATE { get; set; }
        public string COM_ID { get; set; }
        public string CRT_BY { get; set; }
    }
}
