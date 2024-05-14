using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Tổng giá")]
        public decimal TotalPrice => ProductVariant.Quantity * ProductVariant.Price;

        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [ForeignKey("Categories")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey("ProductVariants")]
        public int ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
    }
}
