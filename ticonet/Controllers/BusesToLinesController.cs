using System.Web.Mvc;

namespace ticonet.Controllers
{
    [Authorize]
    public class BusesToLinesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}