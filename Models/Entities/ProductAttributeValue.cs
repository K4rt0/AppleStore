using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore.Models.Entities
{
    public class ProductAttributeValue
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Tên giá trị thuộc tính")]
        public string? Name { get; set; }
        [DisplayName("Giá trị thuộc tính")]
        public string? Value { get; set; }
        [ForeignKey("ProductAttribute")]
        public int ProductAttributeId { get; set; }
        public ProductAttribute? ProductAttribute { get; set; }
        public virtual ICollection<VariantsAttributes>? VariantsAttributes { get; set; }
    }
}
