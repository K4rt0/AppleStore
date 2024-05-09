using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Tên danh mục")]
        public string? Name { get; set; }
        [DisplayName("Hiển thị")]
        public bool Display { get; set; }
        [ForeignKey("Discounts")]
        [DisplayName("Mã giảm giá")]
        public int? DiscountId { get; set; }
        public Discount? Discount { get; set; }
        public List<Product>? Products { get; set; }
    }
}