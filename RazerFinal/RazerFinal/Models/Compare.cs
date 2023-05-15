namespace RazerFinal.Models
{
    public class Compare : BaseEntity
    {
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int? CategoryId { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public double? Price { get; set; }
    }
}
