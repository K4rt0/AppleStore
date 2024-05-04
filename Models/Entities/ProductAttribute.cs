using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class ProductAttribute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Tên thuộc tính")]
        public string? Name { get; set; }
        [DisplayName("Mô tả")]
        public string? Description { get; set; }
        public virtual ICollection<ProductAttributeValue>? ProductAttributeValues { get; set; }
    }
}
