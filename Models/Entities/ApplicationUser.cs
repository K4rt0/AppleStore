using Microsoft.AspNetCore.Identity;

namespace AppleStore.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool Active { get; set; }
        public bool Gender { get; set; }
        public DateOnly Birthdate { get; set; }
        public string? Address { get; set; }
        public virtual ICollection<DeliveryAddress>? DeliveryAddresses { get; set; }
    }
}
