using System.ComponentModel.DataAnnotations;

namespace RazerFinal.Models
{
    public class Address : BaseEntity
    {
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        [StringLength(100)]
        public string? Country { get; set; }
        [StringLength(100)]
        public string? City { get; set; }
        [StringLength(100)]
        public string? State { get; set; }
        [StringLength(100)]
        public string? PostalCode { get; set; }
        [StringLength(100)]
        public string? DirectAddress { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public bool IsMain { get; set; }
    }
}
