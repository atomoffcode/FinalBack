using RazerFinal.Models;
using RazerFinal.ViewModels.BasketViewModels;

namespace RazerFinal.Interfaces
{
    public interface ILayoutService
    {
        Task<List<BasketVM>> GetBasket();
        Task<IDictionary<string, string>> GetSettings();

        Task<AppUser> GetUser();
    }
}
