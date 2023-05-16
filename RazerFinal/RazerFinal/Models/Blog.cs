using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RazerFinal.Models
{
    public class Blog : BaseEntity
    {
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        [StringLength(255)]
        public string? Title { get; set; }
        [StringLength(10000)]
        public string? MainDescription { get; set; }
        [StringLength(255)]
        public string? Image { get; set; }
        public List<Comment>? Comments { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }

    }
}
