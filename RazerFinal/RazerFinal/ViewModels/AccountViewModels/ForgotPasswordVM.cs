using System.ComponentModel.DataAnnotations;

namespace RazerFinal.ViewModels.AccountViewModels
{
    public class ForgotPasswordVM
    {
        [EmailAddress]
        public string? Email { get; set; }
    }
}
