﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int CartProductQuantity { get; set; }
        [ForeignKey("AspNetUsers")]
        public string? UserId { get; set; }
        [ForeignKey("ProductVariants")]
        public int ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
    }
}
