namespace RazerFinal.Models
{
    public class ProductSpec : BaseEntity
    {
        public int CategorySpecId { get; set; }
        public CategorySpec CategorySpec { get; set; }
        public int SpecificationId { get; set; }
        public Specification Specification { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
