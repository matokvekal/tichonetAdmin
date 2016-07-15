using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business_Logic.Helpers;
using System.Globalization;
using log4net;
using Newtonsoft.Json;

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