using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business_Logic.Helpers;
using System.Globalization;

namespace ticonet.Controllers
{
    public class homeController : Controller
    {
        // GET: home
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.CenterLat = MapHelper.CenterLat.ToString(CultureInfo.InvariantCulture);
            ViewBag.CenterLng = MapHelper.CenterLng.ToString(CultureInfo.InvariantCulture);
            ViewBag.Zoom = MapHelper.Zoom.ToString();
            ViewBag.TimeForLoad = BusHelper.TimeForLoad;
            return View();
        }
    }
}