using System.ComponentModel.DataAnnotations;

namespace RazerFinal.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool isDeleted { get; set; }


        [StringLength(255)]
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }


        [StringLength(255)]
        public string? UpdatedBy { get; set; }
        public Nullable<DateTime> UpdatedAt { get; set; }


        [StringLength(255)]
        public string? DeletedBy { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }
    }
}
