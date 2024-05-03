using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppleStore.Models.Entities;

namespace AppleStore
{
    public class ProductVariant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int ColorId { get; set; }
        [DisplayName("Màu sắc")]
        public ProductAttributeValue? Color { get; set; }
        public int DisplaySizeId { get; set; }

        [DisplayName("Kích thước màn hình")]
        public ProductAttributeValue? DisplaySize { get; set; }

        public int ResolutionId { get; set; }
        [DisplayName("Độ phân giải")]
        public ProductAttributeValue? Resolution { get; set; }

        public int ProcessorId { get; set; }
        [DisplayName("Độ xử lý")]
        public ProductAttributeValue? Processor { get; set; }

        public int MemoryId { get; set; }
        [DisplayName("Bộ nhớ")]
        public ProductAttributeValue? Memory { get; set; }

        public int StorageCapacityId { get; set; }
        [DisplayName("Dung lượng lưu trữ")]
        public ProductAttributeValue? StorageCapacity { get; set; }

        public int CameraId { get; set; }
        [DisplayName("Chụp ảnh")]
        public ProductAttributeValue? Camera { get; set; }

        public int BatteryId { get; set; }
        [DisplayName("Dung lượng pin")]
        public ProductAttributeValue? Battery { get; set; }

        public int ConnectivityId { get; set; }
        [DisplayName("Kết nối")]
        public ProductAttributeValue? Connectivity { get; set; }

        public int OperatingId { get; set; }
        [DisplayName("Hệ điều hành")]
        public ProductAttributeValue? Operating { get; set; }

        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}