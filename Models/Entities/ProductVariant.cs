using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public virtual ICollection<VariantsAttributes>? VariantsAttributes { get; set; }
    }
}