﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace AppleStore.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [DisplayName("Họ tên")]
        public string? FullName { get; set; }
        [DisplayName("Ảnh đại diện")]
        public string? Avatar { get; set; }
        public bool Active { get; set; }
        [DisplayName("Giới tính")]
        public bool Gender { get; set; }
        [DisplayName("Ngày sinh")]
        public DateOnly Birthdate { get; set; }
        [DisplayName("Địa chỉ")]
        public string? Address { get; set; }
        public virtual ICollection<DeliveryAddress>? DeliveryAddresses { get; set; }
    }
}
