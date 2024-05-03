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
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

    }
}
