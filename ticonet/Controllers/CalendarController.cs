using System.Web.Mvc;

namespace ticonet.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}