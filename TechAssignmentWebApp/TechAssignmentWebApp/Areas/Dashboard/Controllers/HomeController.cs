using System.Web.Mvc;

namespace TechAssignmentWebApp.Areas.Dashboard.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}