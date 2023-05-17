using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace RazerFinal.ViewModels.AccountViewModels
{
    public class PasswordVM
    {
        public string? Id { get; set; }
        public string? Token { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
