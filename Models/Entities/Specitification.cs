using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppleStore
{
    public class Specitification
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Thông số")]
        public string? Name { get; set; }
        [DisplayName("Mô tả")]
        public string? Description { get; set; }
    }
}