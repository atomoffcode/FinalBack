using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RazerFinal.Models
{
    public class Setting
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string? Key { get; set; }
        [StringLength(10000)]
        public string? Value { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
    }
}
