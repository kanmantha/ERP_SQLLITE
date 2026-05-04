using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Web.Models {
  public class Order
  {
    public int Id { get; set; }

    [Display(Name = "Customer")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a customer.")]
    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal Total { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
  }
}
