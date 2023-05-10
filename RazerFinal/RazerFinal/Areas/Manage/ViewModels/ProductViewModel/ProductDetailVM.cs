using RazerFinal.Models;

namespace RazerFinal.Areas.Manage.ViewModels.ProductViewModel
{
    public class ProductDetailVM
    {
        public Product Product { get; set; }
        public List<ProductSpec> ProductSpecs { get; set; }
    }
}
