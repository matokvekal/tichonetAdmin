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
    public class homeController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(homeController));

        public ActionResult Index()
        {

            ViewBag.CenterLat = MapHelper.CenterLat.ToString(CultureInfo.InvariantCulture);
            ViewBag.CenterLng = MapHelper.CenterLng.ToString(CultureInfo.InvariantCulture);
            ViewBag.Zoom = MapHelper.Zoom.ToString();
            ViewBag.TimeForLoad = BusHelper.TimeForLoad;
            ViewBag.HiddenLines = JsonConvert.SerializeObject(MapHelper.HiddenLines);
            ViewBag.HiddenStations = JsonConvert.SerializeObject(MapHelper.HiddenStations);
            ViewBag.HiddenStudents = JsonConvert.SerializeObject(MapHelper.HiddenStudents);
            ViewBag.ShowStations = MapHelper.ShowStationsWithoutLine;
            return View();
        }
    }
}