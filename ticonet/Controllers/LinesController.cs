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
    public class LinesController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LinesController));

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetLines(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var lines = new List<GridLineModel>();
            var totalRecords = 0;
            using (var logic = new LineLogic())
            {
                lines = logic.GetPaged(_search, rows, page, sidx, sord, filters)
                    .Select(z => new GridLineModel(z)).ToList();
                using (var busLogic = new tblBusLogic())
                {
                    lines.Where(w => w.BusId > 0).ForEach(x =>
                    {
                        var bus = busLogic.GetBus(x.BusId);
                        x.BusIdDescription = bus != null ? bus.BusId : "";
                    });
                }
                totalRecords = logic.Lines.Count();
            }
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    total = (totalRecords + rows - 1) / rows,
                    page,
                    records = totalRecords,
                    rows = lines
                });
            }
        
        [System.Web.Mvc.HttpPost]
        public JsonResult EditLine(GridLineModel model)
        {
            using (var logic = new LineLogic())
            {
                switch (model.Oper)
                {
                    //case "add":
                    //    logic.SaveLine(model.ToDbModel());
                    //    break;
                    case "edit":
                        var existingLine = logic.GetLine(model.Id);
                        if (existingLine != null)
                        {
                            model.UpdateDbModel(existingLine);
                            logic.SaveChanges();
                            logic.UpdateBusToLine(model.Id, model.BusId);
                        }
                        break;
                    case "del":
                        logic.DeleteLine(model.Id);
                        break;
                }
            }
            return new JsonResult {Data = true};
        }

        public HttpResponseMessage GetExcel(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var lines = new GridLineModel[] {};
            var totalRecords = 0;
            using (var logic = new LineLogic())
            {
                totalRecords = logic.Lines.Count();
                lines = logic.GetPaged(_search, totalRecords, 1, sidx, sord, filters)
                    .Select(z => new GridLineModel(z)).ToArray();
            }

            string Name = "Lines";
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add($"{Name} Sheet");
            worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            worksheet.Cell(1, 1).Value = DictExpressionBuilderSystem.Translate("Line.LineName");
            worksheet.Cell(1, 2).Value = DictExpressionBuilderSystem.Translate("Line.LineNumber");
            worksheet.Cell(1, 3).Value = DictExpressionBuilderSystem.Translate("Line.Direction");
            worksheet.Cell(1, 4).Value = DictExpressionBuilderSystem.Translate("Line.IsActive");
            worksheet.Cell(1, 5).Value = DictExpressionBuilderSystem.Translate("Line.totalStudents");
            worksheet.Cell(1, 6).Value = DictExpressionBuilderSystem.Translate("Line.Duration");
            worksheet.Cell(1, 7).Value = DictExpressionBuilderSystem.Translate("Line.Sun");
            worksheet.Cell(1, 8).Value = DictExpressionBuilderSystem.Translate("Line.Mon");
            worksheet.Cell(1, 9).Value = DictExpressionBuilderSystem.Translate("Line.Tue");
            worksheet.Cell(1, 10).Value = DictExpressionBuilderSystem.Translate("Line.Wed");
            worksheet.Cell(1, 11).Value = DictExpressionBuilderSystem.Translate("Line.Thu");
            worksheet.Cell(1, 12).Value = DictExpressionBuilderSystem.Translate("Line.Fri");
            worksheet.Cell(1, 13).Value = DictExpressionBuilderSystem.Translate("Line.Sut");
            worksheet.Cell(1, 14).Value = DictExpressionBuilderSystem.Translate("Line.BusId");

            for (int i = 0; i < lines.Length; i++)
            {
                var row = 2 + i;
                worksheet.Cell(row, 1).Value = lines[i].LineName;
                worksheet.Cell(row, 2).Value = lines[i].LineNumber;
                worksheet.Cell(row, 3).Value = lines[i].Direction;
                worksheet.Cell(row, 4).Value = lines[i].IsActive;
                worksheet.Cell(row, 5).Value = lines[i].totalStudents;
                worksheet.Cell(row, 6).Value = lines[i].Duration;
                worksheet.Cell(row, 7).Value = lines[i].Sun;
                worksheet.Cell(row, 8).Value = lines[i].Mon;
                worksheet.Cell(row, 9).Value = lines[i].Tue;
                worksheet.Cell(row, 10).Value = lines[i].Wed;
                worksheet.Cell(row, 11).Value = lines[i].Thu;
                worksheet.Cell(row, 12).Value = lines[i].Fri;
                worksheet.Cell(row, 13).Value = lines[i].Sut;
                worksheet.Cell(row, 14).Value = lines[i].BusId;
            }

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


        public GridLineModel[] GetPrint(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var lines = new GridLineModel[] { };
            var totalRecords = 0;
            using (var logic = new LineLogic())
            {
                totalRecords = logic.Lines.Count();
                lines = logic.GetPaged(_search, totalRecords, 1, sidx, sord, filters)
                    .Select(z => new GridLineModel(z)).ToArray();
            }

            return lines;
        }

        public JsonResult GetAvailableBuses(int lineId)
        {
            var buses = new List<SelectItemModel>();
            buses.Add(new SelectItemModel { Value = "0", Text = string.Empty, Title = string.Empty});
            using (var logic = new LineLogic())
            {
                buses.AddRange(logic.GetAvailableBuses(lineId)
                    .Select(z => new SelectItemModel
                    {
                        Value = z.Id.ToString(),
                        Text = z.BusId,
                        Title = string.Format("{0} ({1})", z.BusId, z.BusCompany!=null?z.BusCompany.companyName:string.Empty)
                    }).ToList());
            }

            return new JsonResult { Data = buses };
        }
    }
}