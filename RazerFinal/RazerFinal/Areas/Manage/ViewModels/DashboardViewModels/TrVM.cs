using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazerFinal.Areas.Manage.ViewModels.DashboardViewModels
{
    public class TrVM
    {
        public string? CategoryName { get; set; }
        public int? OrderCount { get; set; }
        [Column(TypeName = "money")]
        public double? TotalSale { get; set; }
    }
}
