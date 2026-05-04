using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using ERP.Web.Data;
using ERP.Web.ViewModels;

namespace ERP.Web.Controllers {
  [Authorize]
  public class DashboardController : Controller
  {
    private readonly ERPDbContext _db;
    public DashboardController(ERPDbContext db) { _db = db; }

    public async Task<IActionResult> Index()
    {
      var model = new DashboardViewModel
      {
        TotalCustomers = await _db.Customers.CountAsync(),
        TotalProducts = await _db.Products.CountAsync(),
        TotalOrders = await _db.Orders.CountAsync(),
        Revenue = (decimal)await _db.OrderItems.SumAsync(oi => (double)(oi.Quantity * oi.UnitPrice))
      };
      return View(model);
    }
  }
}
