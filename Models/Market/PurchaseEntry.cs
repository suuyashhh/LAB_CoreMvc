using System;
using System.Collections.Generic;

namespace Models.Market
{
    public class PurchaseEntry
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string? HotelName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public DateTime Date { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public decimal PaidAmount { get; set; }
        public string? PaymentImage { get; set; }
        public decimal GrandTotal { get; set; }
        public string? Notes { get; set; }
        
        public List<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    }
}
