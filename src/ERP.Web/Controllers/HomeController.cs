using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Controllers {
  public class HomeController : Controller {
    public IActionResult Index() {
      // Simple default response to verify app runs
      return Content("ERP Web App Running");
    }
  }
}
