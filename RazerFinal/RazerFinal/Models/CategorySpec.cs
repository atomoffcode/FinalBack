using System.ComponentModel.DataAnnotations;

namespace RazerFinal.Models
{
    public class CategorySpec : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }
        public List<Specification>? Specifications { get; set; } 
    }
}
