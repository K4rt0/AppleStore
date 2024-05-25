using AppleStore.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppleStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
        public DbSet<VariantsAttributes> VariantsAttributes { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<NewsOnTop> NewsOnTops { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
    }
}
