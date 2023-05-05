using RazerFinal.Helpers;
using RazerFinal.Models;

namespace RazerFinal.ViewModels.StoreViewModels
{
    public class StoreVM
    {
        public Product? Product { get; set; }
        public List<Product>? Products { get; set; }

        public IEnumerable<Category>? Categories { get; set; }
        public IEnumerable<CategorySpec>? SpecCategories { get; set; }
    }
}
