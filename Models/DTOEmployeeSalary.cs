using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTOEmployeeSalary
    {
        public Int64 EMP_TRN_ID { get; set; }
        public Int64 EMP_ID { get;set; }
        public decimal EMP_PRICE { get; set; }
        public string DATE { get; set; }
        public string COM_ID { get; set; }
        public string CRT_BY { get; set; }
    }
}
