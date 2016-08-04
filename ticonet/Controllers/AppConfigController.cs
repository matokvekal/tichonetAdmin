using System.Collections.Generic;
using System.Web.Mvc;
using Business_Logic;

namespace ticonet.Controllers
{
    public class AppConfigController : GenericApiAndMVCController {

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult Update (IDictionary<string,object> settings) {
            using (var l = new tblSettingLogic()) {
                l.UpdateConfig(settings);
            }
            return MakeSuccesResult(true);
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult Get (string [] settings) {
            if (settings == null) return MakeBadRequest();
            using (var l = new tblSettingLogic()) {
                var result = l.GetConfig(settings);
                return MakeSuccesResult(result);
            }
        }
    }
}