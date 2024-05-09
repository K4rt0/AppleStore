using AppleStore.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace AppleStore.Models.Entities
{
    public class Discount
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Active { get; set; }
        public string? Code { get; set; }
        public DateTime Expire { get; set; }
        public int Quantity { get; set; }
        public int Percent { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public ICollection<Category>? Categories { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}