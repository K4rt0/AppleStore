using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}