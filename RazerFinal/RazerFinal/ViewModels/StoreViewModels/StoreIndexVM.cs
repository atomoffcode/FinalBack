using RazerFinal.Models;

namespace RazerFinal.ViewModels.StoreViewModels
{
    public class StoreIndexVM
    {
        public List<Category>? Categories { get; set; }
        public List<Product>? ExlusiveProducts { get; set; }
        public List<Product>? NewProducts { get; set; }
        public List<Product>? DiscountedProducsts { get; set; }
        public List<Slider>? Sliders { get; set; }

    }
}
