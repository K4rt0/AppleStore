using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("SKU")]
        public string Name { get; set; }
        public string? Color { get; set; }
        public string? ColorHex { get; set; }
        [DisplayName("Dung lượng")]
        public string? Storage { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}