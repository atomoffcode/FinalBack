using RazerFinal.Models;

namespace RazerFinal.ViewModels.OrderViewModels
{
    public class OrderVM
    {
        public Order Order { get; set; }
        public List<Basket> Baskets { get; set; }
    }
}
