using System.ComponentModel.DataAnnotations;

namespace AppleStore.Models.Entities
{
    public class NewsOnTop
    {
        [Key]
        public int Id { get; set; }
        public string? Header { get; set; }
        public string? SubHeader { get; set; }
        public string? Content { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
    }
}
