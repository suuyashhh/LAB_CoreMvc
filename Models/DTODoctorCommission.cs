using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTODoctorCommission
    {
        public Int64 DOC_COM_ID { get; set; }
        public Int64 DOCTOR_ID { get; set; }
        public decimal DOC_COM_PRICE { get; set; }
        public string DATE { get; set; }
        public string COM_ID { get; set; }
        public string CRT_BY { get; set; }
    }
}
