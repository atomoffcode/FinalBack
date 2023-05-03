namespace RazerFinal.Models
{
    public class ProductImage : BaseEntity
    {
        public string Iamge { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
