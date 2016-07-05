using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using Business_Logic;
using ClosedXML.Excel;
using log4net;
using ticonet.Models;

namespace ticonet.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BusesController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BusesController));

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetBuses(bool _search, int rows, int page, string sidx, string sord, string filters = "")
        {
            var buses = new List<BusModel>();
            var totalRecords = 0;
            using (var logic = new tblBusLogic())
            {
                buses = logic.GetPaged(_search, rows, page, sidx, sord, filters)
                    .Select(z => new BusModel(z)).ToList();
                totalRecords = logic.Buses.Count();
            }
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    total = (totalRecords + rows - 1) / rows,
                    page,
                    records = totalRecords,
                    rows = buses
                });
            }
        
        [System.Web.Mvc.HttpPost]
        public JsonResult EditBus(BusModel model)
        {
            using (var logic = new tblBusLogic())
            {
                switch (model.Oper)
                {
                    case "add":
                        logic.SaveBus(model.ToDbModel());
                        break;
                    case "edit":
                        logic.Update(model.ToDbModel());
                        break;
                    case "del":
                        logic.DeleteBus(model.Id);
                        break;
                }
            }
            return new JsonResult {Data = true};
        }

        public HttpResponseMessage GetExcel(bool _search, int rows, int page, string sidx, string sord, string filters = "")
        {
            var buses = new List<BusModel>();
            var totalRecords = 0;
            using (var logic = new tblBusLogic())
            {
                totalRecords = logic.Buses.Count();
                buses = logic.GetPaged(_search, totalRecords, 1, sidx, sord, filters)
                    .Select(z => new BusModel(z)).ToList();
            }

            string Name = "Buses";
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add($"{Name} Sheet");
            worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            //buses

            worksheet.Cell(1, 1).Value = "Test 1";
            worksheet.Cell(1, 2).Value = 1;
            worksheet.Cell(2, 1).Value = "Test 2";
            worksheet.Cell(2, 2).Value = 2;
            worksheet.Cell(3, 1).Value = "Test 3";
            worksheet.Cell(3, 2).Value = 3;
            worksheet.Cell(4, 1).Value = "Test 4";
            worksheet.Cell(4, 2).Value = 4;
            worksheet.Rows(2, 4).Group();
            worksheet.Cell(5, 1).Value = "Total";
            worksheet.Cell(5, 2).SetFormulaA1("=SUM(B1:B4)"); ;
            worksheet.CollapseRows(1);
            worksheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.None;
            worksheet.Columns().AdjustToContents();

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                result.Content = new ByteArrayContent(memoryStream.GetBuffer());
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{Name}.xlsx"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            return result;
        }
    }
}