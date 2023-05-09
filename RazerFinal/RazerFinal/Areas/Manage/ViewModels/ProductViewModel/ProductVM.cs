
using RazerFinal.Models;

namespace RazerFinal.Areas.Manage.ViewModels.ProductViewModel
{
    public class ProductVM
    {
        public List<Product>? Products { get; set; }
        public ProductSpec? Spec { get; set; }
        public List<CategorySpec>? CategorySpecs { get; set; }
        public List<Specification>? Specifications { get; set; }
    }
}
