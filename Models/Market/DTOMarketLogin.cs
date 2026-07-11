using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Models.Market
{
    public class DTOMarketLogin
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string contact { get; set; }
        public string pass { get; set; }
    }
}
