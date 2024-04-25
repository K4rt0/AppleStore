using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore {
    public class ProductDetail {
        [Key]
        public int Id { get; set; }

        [DisplayName("Kích thước màn hình")]
        public string? DisplaySize { get; set; }

        [DisplayName("Độ phân giải")]
        public string? Resolution { get; set; }

        [DisplayName("Độ xử lý")]
        public string? Processor { get; set; }
        
        [DisplayName("Bộ nhớ")]
        public string? Memory { get; set; }
        
        [DisplayName("Dung lượng lưu trữ")]
        public string? StorageCapacity { get; set; }

        [DisplayName("Chụp ảnh")]
        public string? Camera { get; set; }

        [DisplayName("Dung lượng pin")]
        public string? Battery { get; set; }

        [DisplayName("Kết nối")]
        public string? Connectivity { get; set; }

        [DisplayName("Hệ điều hành")]
        public string? Operating { get; set; }

        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}