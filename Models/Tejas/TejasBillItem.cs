using System;

namespace Models.Tejas
{
    public class TejasBillItem
    {
        public int Id { get; set; }
        public string? BillId { get; set; }
        public string? FoodId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Image { get; set; }
    }
}
