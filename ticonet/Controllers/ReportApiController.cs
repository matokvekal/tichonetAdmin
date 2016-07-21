using Business_Logic;
using Business_Logic.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ticonet.Helpers;

namespace ticonet.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ReportApiController : ApiController{

        [System.Web.Mvc.HttpGet]
        public JsonResult GetLinesTotalStatistic(int year) {
            DateTime date = new DateTime(year, 1, 1);
            var result = new List<LinesTotalStatisticDto>();
            using (var logic = new LineLogic()) {
                for (int i = 0; i < 12; i++) 
                    result.Add(logic.GetLinesTotalStatistic(date.AddMonths(i), date.AddMonths(i + 1)));
            }
            return new JsonResult { Data = result };
        }

        public HttpResponseMessage GetLinesTotalStatisticXL(int year) {
            using (var l = new LineLogic()) {
                DateTime date = new DateTime(year, 1, 1);
                var data = new List<LinesTotalStatisticDto>();
                for (int i = 0; i < 12; i++)
                    data.Add(l.GetLinesTotalStatistic(date.AddMonths(i), date.AddMonths(i + 1)));
                return ExcellWriter.BookToHTTPResponseMsg(
                    ExcellWriter.AllLinesTotalStatistic(data),
                    "Summary Lines Report (" + (new DateTime(year, 1, 1)).ToString("yyyy")+")"
                    );
            }
        }


        [System.Web.Mvc.HttpGet]
        public JsonResult GetAllLinesPeriodStatistic (DateTime startDate, DateTime endDate){
            using (var l = new LineLogic()) {
                var rows = l.GetAllLinesPeriodActivities(startDate, endDate);
                var footer = l.GetLineTotalStatisticByDays(startDate, endDate);
                return new JsonResult { Data = new { rows = rows, footer = footer } };
            }
        }

        public HttpResponseMessage GetAllLinesPeriodStatisticXL (DateTime startDate, DateTime endDate) {
            using (var l = new LineLogic()) {
                var data = l.GetAllLinesPeriodActivities(startDate, endDate);
                return ExcellWriter.BookToHTTPResponseMsg(
                    ExcellWriter.AllLinesPeriodStatistic(data),
                    "Lines Report (" + startDate.ToString("dd-MM-yyyy") + " - " + endDate.ToString("dd-MM-yyyy") + ")"
                    );
            }
        }
    }
}
