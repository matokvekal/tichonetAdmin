using System.Web.Mvc;

namespace ticonet.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}