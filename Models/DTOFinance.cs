using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTOFinance
    {
        public int TURNOVER { get; set; }
        public int LAB_MATERIALS { get; set; }
        public int BIKE_FUEL { get; set; }
        public int EMPLOYEE_SALARY { get; set; }
        public int ELECTRICITY_BILL { get; set; }
        public int OTHER_EXPENSE { get; set; }
        public int DOCTOR_COMMISSION { get; set; }
        public int TOTAL_EXPENSE { get; set; }
        public int NET_AMOUNT { get; set; }
    }
}
