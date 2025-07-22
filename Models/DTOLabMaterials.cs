using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class DTOLabMaterials
    {
        public Int64 MAT_ID { get; set; }
        public string MAT_NAME { get; set; }
        public decimal MAT_PRICE { get; set; }
        public string DATE { get; set; }
        public string COM_ID { get; set; }
        public string CRT_BY {  get; set; }
    }
}
