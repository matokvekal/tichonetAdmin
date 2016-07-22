using Business_Logic;
using Business_Logic.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ticonet.Helpers;
using DEBS = Business_Logic.DictExpressionBuilderSystem;


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

        [System.Web.Mvc.HttpGet]
        public JsonResult GetAllLinesPeriodStatistic (DateTime startDate, DateTime endDate){
            using (var l = new LineLogic()) {
                var rows = l.GetAllLinesPeriodActivities(startDate, endDate);
                var footer = l.GetLineTotalStatisticByDays(startDate, endDate);
                return new JsonResult { Data = new { rows = rows, footer = footer } };
            }
        }

        public HttpResponseMessage GetReportXL (DateTime startDate, DateTime endDate, int summaryYear) {
            using (var l = new LineLogic()) {
                var data = l.GetAllLinesPeriodActivities(startDate, endDate);
                DateTime date = new DateTime(summaryYear, 1, 1);
                var dataSummary = new List<LinesTotalStatisticDto>();
                for (int i = 0; i < 12; i++)
                    dataSummary.Add(l.GetLinesTotalStatistic(date.AddMonths(i), date.AddMonths(i + 1)));

                var book = ExcellWriter.NewBook();
                var sheet = ExcellWriter.NewReportSheet(book, DEBS.Translate("Lines.Report"), "Lines Report from " + startDate.ToString("dd-MM-yyyy") + " to " + endDate.ToString("dd-MM-yyyy"), 2);
                int row = ExcellWriter.AddLinesPeriodStatisticToSheet(sheet, 4,
                    l.GetAllLinesPeriodActivities(startDate, endDate),
                    l.GetLineTotalStatisticByDays(startDate, endDate)
                    );
                ExcellWriter.AddLinesSummaryStatisticToSheet(sheet, row + 1, dataSummary);
                return ExcellWriter.BookToHTTPResponseMsg(
                    book,
                    "Lines Report (" + startDate.ToString("dd-MM-yyyy") + " - " + endDate.ToString("dd-MM-yyyy") + ")"
                    );
            }
        }
    }
}
