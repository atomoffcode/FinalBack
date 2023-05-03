using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RazerFinal.Models
{
    public class Product : BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; }
        [Column(TypeName = "money")]
        public double Price { get; set; }
        [Column(TypeName = "money")]

        public double DiscountedPrice { get; set; }
        [Column(TypeName = "money")]

        public double ExTax { get; set; }
        public int Count { get; set; }
        [StringLength(6)]
        public string? Seria { get; set; }
        public int? Code { get; set; }
        public string? BasicInfo { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }
        [StringLength(255)]
        public string? MainImage { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductSpec>? Specifications { get; set; }
        public IEnumerable<Basket>? Baskets { get; set; }


        [NotMapped]
        public IFormFile? MainFile { get; set; }
        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
    }
}
