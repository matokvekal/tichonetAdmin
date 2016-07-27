using System.Web.Mvc;

namespace ticonet.Controllers
{
    [Authorize]
    public class LinesController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}