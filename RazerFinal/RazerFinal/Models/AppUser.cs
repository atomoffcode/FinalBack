using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazerFinal.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]

        public string? SurName { get; set; }
        [StringLength(255)]
        public string? ProfileImage { get; set; }
        public List<Basket>? Baskets { get; set; }
        public List<Compare>? Compares { get; set; }

        public List<Address>? Addresses { get; set; }
        public List<Order>? Orders { get; set; }
        public List<Comment>? Comments { get; set; }
        public List<UserToken>? Tokens { get; set; } 

        public bool IsActive { get; set; }
        [NotMapped]
        public IFormFile? ProfileImageFile { get; set; }

    }
}
