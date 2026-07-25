using System;

namespace Models.BillingApp
{
    public class DTOProductEntries
    {
        public long product_id { get; set; }
        public string english_name { get; set; } = string.Empty;
        public string marathi_name { get; set; } = string.Empty;
        public string quantity { get; set; } = string.Empty;
        public decimal price { get; set; }
        public long? barcodeNo { get; set; }
    }
}
