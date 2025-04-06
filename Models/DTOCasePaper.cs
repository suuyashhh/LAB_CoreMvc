using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTOCasePaper
    {
        public Int64 TRN_NO { get; set; }
        public string PATIENT_NAME { get; set; }
        public string GENDER { get; set; }
        public string CON_NUMBER { get; set; }
        public string ADDRESS { get; set; }
        public string DOCTOR_REF { get; set; }
        public string DATE { get; set; }
        public int STATUS_CODE { get; set; }
        public string SHORT_TRN_NO { get; set; }
        public decimal TOTAL_AMOUNT { get; set; }
        public decimal TOTAL_PROFIT { get; set; }
        public decimal DISCOUNT { get; set; }
        public string DELETE_REASON { get; set; }
    }
}
