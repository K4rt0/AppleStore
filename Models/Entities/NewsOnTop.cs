using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppleStore.Models.Entities
{
    public class NewsOnTop
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Tiêu đề")]
        public string? Header { get; set; }
        [DisplayName("Phụ đề")]
        public string? SubHeader { get; set; }
        [DisplayName("Nội dung")]
        public string? Content { get; set; }
        [DisplayName("Giá tiền")]
        public decimal Price { get; set; }
        [DisplayName("Hình ảnh")]
        public string? Image { get; set; }
    }
}
