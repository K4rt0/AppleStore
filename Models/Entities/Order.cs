using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Confirmed { get; set; }
        public bool Paid { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Note { get; set; }
        [ForeignKey("Discounts")]
        public int DiscountId { get; set; }
        public Discount? Discount { get; set; }
        [ForeignKey("ApplicationUsers")]
        public string? UserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
