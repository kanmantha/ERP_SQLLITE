using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERP.Web.Models
{
    /// <summary>
    /// Represents a Stock Movement (In or Out) for inventory tracking.
    /// </summary>
    public class StockMovement
    {
        public int Id { get; set; }

        [Display(Name = "Product")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Range(-10000, 10000)]
        public int Quantity { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; } = "In"; // "In", "Out", "Adjustment"

        [Display(Name = "Reason")]
        public string Reason { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}
