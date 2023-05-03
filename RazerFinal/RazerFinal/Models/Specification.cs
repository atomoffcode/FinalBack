using System.ComponentModel.DataAnnotations;

namespace RazerFinal.Models
{
    public class Specification : BaseEntity
    {
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
        public int? CategorySpecId { get; set; }

        public CategorySpec? CategorySpec { get; set; }
    }
}
