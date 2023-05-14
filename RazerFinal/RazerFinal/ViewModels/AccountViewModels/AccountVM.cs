using Org.BouncyCastle.Asn1.Mozilla;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazerFinal.ViewModels.AccountViewModels
{
    public class AccountVM
    {
        [StringLength(255)]
        public string? Id { get; set; }
        [StringLength(100)]
        public string? Name { get; set; }
        [StringLength(100)]
        public string? SurName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
        public string? UserName { get; set; }
        [StringLength(255)]
        public string? ProfileImage { get; set; }
        [NotMapped]
        public IFormFile? ProfileImageFile { get; set; }
    }
}
