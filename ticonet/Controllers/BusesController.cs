using System.Web.Mvc;

namespace ticonet.Controllers
{
    [Authorize]
    public class BusesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}