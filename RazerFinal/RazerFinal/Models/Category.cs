using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazerFinal.Models
{
    public class Category : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        public IEnumerable<Product>? Products { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
