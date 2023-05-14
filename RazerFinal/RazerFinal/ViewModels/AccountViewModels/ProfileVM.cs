using RazerFinal.Models;

namespace RazerFinal.ViewModels.AccountViewModels
{
    public class ProfileVM
    {
        public List<Address>? Addresses { get; set; }
        public Address? Address { get; set; }
        public AppUser? AppUser { get; set; }

        public List<Order>? Orders { get; set; }
        public AccountVM? AccountVM { get; set; }
    }
}
