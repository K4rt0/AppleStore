using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppleStore
{
    public class SpecitificationValue
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        [ForeignKey("Specitifications")]
        public int SpecitificationId { get; set; }
        public Specitification? Specitification { get; set; }
    }
}