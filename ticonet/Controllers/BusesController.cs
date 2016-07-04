using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Http;
using Business_Logic;
using log4net;
using ticonet.Models;

namespace ticonet.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BusesController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BusesController));

        [System.Web.Mvc.HttpGet]
        public JsonResult GetBuses()
        {
            var buses = new List<BusModel>();
            using (var logic = new tblBusLogic())
            {
                buses = logic.GetList().Select(z => new BusModel(z)).ToList();
            }
            return new JsonResult {Data = buses};
        }
        
        [System.Web.Mvc.HttpPost]
        public JsonResult SaveBus(BusModel bus)
        {
            //using (var logic = new tblBusLogic())
            //{
            //    buses = logic.GetList().Select(z => new BusModel(z)).ToList();
            //}
            return new JsonResult {Data = true};
        }
    }
}