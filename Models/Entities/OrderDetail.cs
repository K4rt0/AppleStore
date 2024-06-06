using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("Orders")]
        public int OrderId { get; set; }
        public virtual Order? Order { get; set; }
        [ForeignKey("ProductVariants")]
        public int ProductVariantId { get; set; }
        public virtual ProductVariant? ProductVariant { get; set; }

    }
}
