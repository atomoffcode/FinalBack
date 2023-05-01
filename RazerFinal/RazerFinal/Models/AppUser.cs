using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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

        public bool IsActive { get; set; }
    }
}
