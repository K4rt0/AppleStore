using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}