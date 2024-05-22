using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class DeliveryAddress
    {
        [Key]
        public int Id { get; set; }
        public string? FullName { get; set; }
        public int PhoneNumber { get; set; }
        public string? Address { get; set; }
        public bool Default { get; set; }
        [ForeignKey("AspNetUsers")]
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
