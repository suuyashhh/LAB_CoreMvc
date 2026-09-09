using System;

namespace Models.Tejas
{
    public class DTOTejasShop
    {
        public long TEJAS_SHOPES_ID { get; set; }
        public string SHOP_NAME { get; set; } = string.Empty;
        public string? SHOP_CODE { get; set; }
        public string? ADDRESS { get; set; }
        public string? CONTACT { get; set; }
        public string? EMAIL { get; set; }
        public string? GST_NO { get; set; }
        public string? LOGO_URL { get; set; }
        public string? ACTIVE { get; set; } = "Y";
        public DateTime? CREATED_AT { get; set; }
        public DateTime? UPDATED_AT { get; set; }
    }
}
