using System.Web.Mvc;
using log4net;
using Business_Logic.SqlContext;
using ticonet.Controllers.Ng;
using Newtonsoft.Json;
using Business_Logic.MessagesModule;
using ticonet.ParentControllers;
using Business_Logic.MessagesModule.Mechanisms;
using System.Linq;
using Business_Logic.MessagesModule.EntitiesExtensions;
using System;

namespace ticonet.Controllers{
    [Authorize]
    public class HomeController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(HomeController));

        private readonly ISqlLogic _sqlLogic;

        public HomeController(ISqlLogic sqlLogic){
            _sqlLogic = sqlLogic;
        }

        class Testmsg : IEmailMessage {
            public string Body { get { return "Hello World"; } }
            public bool IsBodyHtml { get { return false; } }
            public string RecepientAdress { get; set; }
            public string RecepientName { get; set; }
            public string Subject { get { return "test"; } }
        }

        public ActionResult Index(){

            //Business_Logic.MessagesModule.Mechanisms.TASK_PROTOTYPE.RunBatchCreation(int.MaxValue, _sqlLogic);

            //using (var l = new MessagesModuleLogic()) {
            //    var prov = l.GetAll<tblEmailSenderDataProvider>().First();
            //    var sender = new EmailSender();
            //    var testmsg = new Testmsg { RecepientAdress = "budebova@stexsy.com", RecepientName = "dontmatter" };
            //    sender.SendSingle(testmsg, prov);
            //}
            
            ;
            return RedirectToAction("Index", "MessageModule"); //, new { area = "Messages" }
        }
    }
}