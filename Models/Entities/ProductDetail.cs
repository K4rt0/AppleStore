using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore {
    public class ProductDetail {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [ForeignKey("SpecitificationValues")]
        public int SpecitificationValueId { get; set; }
        public SpecitificationValue? SpecitificationValue { get; set; }
    }
}