namespace Models.Market
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public int VegetableId { get; set; }
        public string? VegetableName { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal Total { get; set; }
    }
}
