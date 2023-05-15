using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [NotMapped]
        public string? FullAddress => $"{User.Name} {User.SurName}, {Country}, {City} , {DirectAddress}";
    }
}
