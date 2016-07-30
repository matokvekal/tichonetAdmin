using System.Web.Mvc;

namespace ticonet.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}