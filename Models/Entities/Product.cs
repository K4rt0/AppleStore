using AppleStore.Models.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore
{
        public class Product
        {
                [Key]
                [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
                public int Id { get; set; }
                [DisplayName("Tên sản phẩm")]
                public string? Name { get; set; }
                [DisplayName("Hiển thị")]
                public bool Display { get; set; }
                [DisplayName("Mô tả")]
                public string? Description { get; set; }
                [DisplayName("Hình đại diện")]
                public string? Avatar { get; set; }
                [DisplayName("Bán chạy")]
                public bool HotSeller { get; set; }

                [ForeignKey("Discounts")]
                [DisplayName("Giảm giá")]
                public int? DiscountId { get; set; }
                public Discount? Discount { get; set; }
                [ForeignKey("Categories")]
                [DisplayName("Giảm giá")]
                public int CategoryId { get; set; }
                [DisplayName("Loại sản phẩm")]
                public Category? Category { get; set; }
                [DisplayName("Hình ảnh mô tả")]
                public virtual ICollection<ProductImage>? ProductImages { get; set; }
                public virtual ICollection<ProductVariant>? ProductVariants { get; set; }
                public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
        }
}