using AppleStore.Models.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppleStore.Models.Entities
{
    public class Discount
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Chương trình khuyến mãi")]
        public string? Name { get; set; }
        [DisplayName("Sử dụng")]
        public bool Active { get; set; }
        [DisplayName("Mã giảm giá")]
        public string? Code { get; set; }
        [DisplayName("Thời hạn")]
        public DateTime Expire { get; set; }
        [DisplayName("Số lượng")]
        public int Quantity { get; set; }
        [DisplayName("Phầm trăm")]
        public int Percent { get; set; }
        [DisplayName("Số tiền")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}