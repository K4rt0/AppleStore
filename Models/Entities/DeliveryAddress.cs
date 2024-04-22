using System.ComponentModel.DataAnnotations;

namespace AppleStore.Models.Entities
{
    public class DeliveryAddress
    {
        [Key]
        public int Id { get; set; }
        public string? FullName { get; set; }
        public int PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public int PostalCode { get; set; }
    }
}
