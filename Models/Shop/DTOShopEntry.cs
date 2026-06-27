using System;

namespace Models.Shop
{
    public class DTOShopEntry
    {
        public long SHOP_ENTRY_ID { get; set; }
        public bool IS_PAID { get; set; }
        public string REASON { get; set; }
        public long PRICE { get; set; }
        public long USER_ID { get; set; }
        public string? IMAGE1 { get; set; }
        public string? IMAGE2 { get; set; }
        public string? IMAGE3 { get; set; }
        public string? IMAGE4 { get; set; }
        public DateTime DATE { get; set; }
        public int? EntryType { get; set; }
    }
}
