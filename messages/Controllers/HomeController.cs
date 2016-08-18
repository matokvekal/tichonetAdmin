using System.Web.Mvc;
using log4net;
using Business_Logic.SqlContext;
using ticonet.Controllers.Ng;
using Newtonsoft.Json;

namespace ticonet.Controllers{
    [Authorize]
    public class HomeController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HomeController));

        private readonly ISqlLogic _sqlLogic;

        public HomeController(ISqlLogic sqlLogic){
            _sqlLogic = sqlLogic;
        }

        public ActionResult Index(){
            return RedirectToAction("Index", "MessageModule"); //, new { area = "Messages" }
        }
    }
}