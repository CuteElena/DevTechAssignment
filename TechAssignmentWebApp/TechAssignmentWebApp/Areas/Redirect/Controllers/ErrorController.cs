using TechAssignmentWebApp;
using System.Web.Mvc;

namespace TechAssignmentWebApp.Areas.Redirect.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Redirect/Error/
        public ActionResult PageNotFound()
        {
            return View();
        }
    }
}