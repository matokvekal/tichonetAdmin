using Business_Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using ticonet.Models;
using DEBS = Business_Logic.DictExpressionBuilderSystem;


namespace ticonet.Controllers
{
    public class LinesPlanController : GenericApiAndMVCController {
        const string HARDCODED_PASS = "1234";

        public ActionResult Index()
        {
            return View();
        }

        //SPA API------------------------------------------------

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult Get(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "") {
            using(var l = new tblLinesPlanLogic()) {
                var list = l.GetPaged(_search, rows, page, sidx, sord, filters)
                    .Select(x => {
                        var m = new LinesPlanApiModel {
                            ParentLineName = x.Line.LineName,
                            ParentLineNumber = x.Line.LineNumber,
                            Id = x.Id,
                            LineId = x.LineId
                        };
                        m.SyncDatesTo(x);
                        return m;
                    }
                    )
                     .ToList();
                return MakeJqGridResult(
                    list, rows, l.tblLinesCount, page
                    );
            }
            //return MakeJqGridResult
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult SaveOrUpdate(LinesPlanApiModel model, string pass, bool doAppendPlan) {
            if (pass != HARDCODED_PASS)
                return MakeBadRequest(DEBS.Translate("Message.PasswordInvalid"));
            using (var l = new tblLinesPlanLogic()) {
                if (doAppendPlan) {
                    var tlp = l.GetFirstByLine(model.LineId) ??
                        new tblLinesPlan { LineId = model.LineId };
                    tlp.SyncDatesTo(model);
                    l.Save(tlp);
                }
                else {
                    var tlp = l.GetFirstByLine(model.LineId);
                    if (tlp != null)
                        l.Remove(tlp);
                }
            }
            return MakeSuccesResult();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult Edit(LinesPlanApiModel model) {
            using (var l = new tblLinesPlanLogic()) {
                var item = l.Get(model.Id);
                item.SyncDatesTo(model);
                l.Save(item);
            }
            return MakeSuccesResult(true);
            //return MakeBadRequest();
        }



    }
}