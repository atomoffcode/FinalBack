using System.ComponentModel.DataAnnotations;

namespace RazerFinal.Models
{
    public class Category : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
    }
}
