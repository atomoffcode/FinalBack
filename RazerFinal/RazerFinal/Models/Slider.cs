using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RazerFinal.Models
{
    public class Slider : BaseEntity
    {
        [StringLength(255)]
        public string MainHead { get; set; }
        [StringLength(255)]
        public string SubHead { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
