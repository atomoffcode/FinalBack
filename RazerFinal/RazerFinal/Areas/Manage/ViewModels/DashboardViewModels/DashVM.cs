using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RazerFinal.Enums;
using RazerFinal.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace RazerFinal.Areas.Manage.ViewModels.DashboardViewModels
{
    public class DashVM
    {
        public List<TrVM> Transactions { get; set; }
        public List<Order>? Orders { get; set; }

        public float? PotensialGrowth { get; set; }
        [Column(TypeName = "money")]
        public double? Growth { get; set; }

        [Column(TypeName = "money")]
        public double? Revenue { get; set; }
        public float? RevenuePerc { get; set; }
        [Column(TypeName = "money")]
        public double? Income { get; set; }
        [Column(TypeName = "money")]
        public double? YesIncome { get; set; }
        public float? IncomePerc { get; set; }

        public List<Countries>? Countries { get; set; }

    }
}
