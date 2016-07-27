using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business_Logic;
using ticonet.Models;

namespace ticonet.Controllers
{
    public class AppConfigController : GenericApiAndMVCController {

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult Update (IDictionary<string,object> settings) {
            using (var l = new AppConfigLogic()) {
                l.UpdateConfig(settings);
            }
            return MakeSuccesResult(true);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult Get (string [] settings) {
            if (settings == null) return MakeBadRequest();
            using (var l = new AppConfigLogic()) {
                var result = l.GetConfig(settings);
                return MakeSuccesResult(result);
            }
        }
    }
}