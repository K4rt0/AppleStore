using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppleStore.Models.Entities
{
    public enum OrderStatus
    {
        [Display(Name = "Đang chờ xử lý")]
        Pending,
        [Display(Name = "Đã xác nhận")]
        Confirmed,
        [Display(Name = "Đã vận chuyển")]
        Shipped,
        [Display(Name = "Đã giao")]
        Delivered,
        [Display(Name = "Đã hủy")]
        Cancelled
    }
}
