using System.ComponentModel.DataAnnotations;

namespace AppleStore
{
    public class Discount
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public string? Code { get; set; }
        public int Quantity { get; set; }
        public int Percent { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}