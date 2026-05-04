using System;
using System.ComponentModel.DataAnnotations;

namespace ERP.Web.Models
{
    /// <summary>
    /// Represents an Invoice generated from an Order.
    /// </summary>
    public class Invoice
    {
        public int Id { get; set; }

        [Display(Name = "Invoice No.")]
        public string InvoiceNo { get; set; } = "INV-0001";

        [Display(Name = "Order")]
        public int? OrderId { get; set; }

        public Order? Order { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }

        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        public decimal Amount { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Paid, Cancelled
    }
}
