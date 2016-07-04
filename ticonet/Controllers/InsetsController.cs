using System.Web.Mvc;
using log4net;

namespace ticonet.Controllers
{
    [Authorize]
    public class InsetsController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(InsetsController));
        
        public ActionResult Index()
        {
            return View();
        }
    }
}