using System;

namespace Models.Tejas
{
    public class TejasFoodItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
    }
}
