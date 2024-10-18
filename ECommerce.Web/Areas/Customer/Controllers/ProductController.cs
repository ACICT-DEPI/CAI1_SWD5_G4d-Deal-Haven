using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Web.Areas.Customer.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
