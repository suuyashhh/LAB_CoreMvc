using System;
using System.Collections.Generic;

namespace Models.Tejas
{
    public class TejasBill
    {
        public string Id { get; set; }
        public string BillNumber { get; set; }
        public decimal Subtotal { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<TejasBillItem> Items { get; set; } = new List<TejasBillItem>();
    }
}
