using System.ComponentModel.DataAnnotations;

namespace ERP.Web.Models
{
    /// <summary>
    /// Represents a supplier/vendor for products.
    /// </summary>
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Contact Person")]
        public string ContactName { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
