using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;
using Business_Logic;
using ClosedXML.Excel;
using log4net;
using ticonet.Models;
using Business_Logic.Enums;
using Business_Logic.Helpers;

namespace ticonet.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ScheduleApiController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ScheduleApiController));

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetLines(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var lines = new List<GridLineModel>();
            var totalRecords = 0;
            using (var logic = new LineLogic())
            {
                lines = logic.GetList()
                    .Select(z => new GridLineModel(z)).ToList();
                totalRecords = logic.Lines.Count();
            }
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    //total = (totalRecords + rows - 1) / rows,
                    //page,
                    //records = totalRecords,
                    rows = lines
                });
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetSchedule([FromUri]ScheduleParamsModel parameters, bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var items = new List<ScheduleItemModel>();
            var totalRecords = 0;
            var linesIdsList = parameters.LinesIds != null
                ? parameters.LinesIds.Split(',').Select(int.Parse)
                : null;
            var dateFromDt = DateHelper.StringToDate(parameters.DateFrom);
            var dateToDt = DateHelper.StringToDate(parameters.DateTo);
            using (var logic = new tblScheduleLogic())
            {
                items = logic.GetPaged(linesIdsList, dateFromDt, dateToDt, _search, rows, page, sidx, sord, filters)
                    .Select(z => new ScheduleItemModel(z)).ToList();
                totalRecords = logic.Schedule.Count();
            }
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    total = (totalRecords + rows - 1) / rows,
                    page,
                    records = totalRecords,
                    rows = items
                });
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GenerateSchedule([FromUri]ScheduleParamsModel parameters)
        {
            var dateFrom = DateHelper.StringToDate(parameters.DateFrom);
            var dateTo = DateHelper.StringToDate(parameters.DateTo);
            var schedule = new List<ScheduleItemModel>();
            var fakeId = 0;

            if (!dateFrom.HasValue || !dateTo.HasValue || string.IsNullOrEmpty(parameters.LinesIds))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            using (var logic = new LineLogic())
            {
                var lines = logic.GetLines(parameters.LinesIds.Split(',').Select(int.Parse));

                foreach (var line in lines)
                {
                    var dates = GetScheduleLineDates(line, dateFrom.Value, dateTo.Value, parameters);
                    foreach (var date in dates)
                    {
                        schedule.Add(new ScheduleItemModel
                        {
                            Id = fakeId++,
                            Date = DateHelper.DateToString(date),
                            Direction = (LineDirection)line.Direction,
                            LineId = line.Id,
                            BusId = line.BusesToLines.DefaultIfEmpty(new BusesToLine()).First().BusId,
                            LineIdDescription = ScheduleItemModel.GetLineIdDescription(line),
                            BusIdDescription = ScheduleItemModel.GetBusIdDescription(line.BusesToLines.DefaultIfEmpty(new BusesToLine()).First().Bus),
                            leaveTime = string.Empty,
                            arriveTime = string.Empty
                        });
                    }
                }
            }
            
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    //total = (totalRecords + rows - 1) / rows,
                    //page,
                    //records = totalRecords,
                    rows = schedule
                });
        }

        [System.Web.Http.HttpPost]
        public JsonResult EditItem(ScheduleItemModel model)
        {
            using (var logic = new tblScheduleLogic())
            {
                switch ((GridOperation)Enum.Parse(typeof(GridOperation), model.Oper, true))
                {
                    case GridOperation.add:
                        logic.SaveItem(model.ToDbModel());
                        break;
                    case GridOperation.edit:
                        logic.Update(model.ToDbModel());
                        break;
                    case GridOperation.del:
                        logic.DeleteItem(model.Id);
                        break;
                }
            }
            return new JsonResult { Data = true };
        }
        
        public JsonResult GetScheduleLines()
        {
            var lines = new List<SelectItemModel>();
            lines.Add(new SelectItemModel { Value = "0", Text = string.Empty, Title = string.Empty });
            using (var logic = new LineLogic())
            {
                lines.AddRange(logic.GetList()
                    .Select(z => new SelectItemModel
                    {
                        Value = z.Id.ToString(),
                        Text = z.LineName,
                        Title = string.Format("{0} ({1} - {2})", z.LineName, z.LineNumber, DictExpressionBuilderSystem.Translate("General." + (LineDirection)z.Direction))
                    }).ToList());
            }

            return new JsonResult { Data = lines };
        }
        
        public JsonResult GetScheduleDrivers()
        {
            var drivers = new List<SelectItemModel>();
            drivers.Add(new SelectItemModel { Value = "0", Text = string.Empty, Title = string.Empty });
            using (var logic = new DriverLogic())
            {
                drivers.AddRange(logic.GetList()
                    .Select(z => new SelectItemModel
                    {
                        Value = z.Id.ToString(),
                        Text = z.FirstName + " " + z.LastName,
                        Title = string.Format("{0} - {1}", z.GpsId, z.Company)
                    }).ToList());
            }

            return new JsonResult { Data = drivers };
        }

        public JsonResult GetAvailableBuses(int? lineId = null, int? scheduleId = null)
        {
            var buses = new List<SelectItemModel>();
            Line scheduleLine = null;
            buses.Add(new SelectItemModel { Value = "0", Text = string.Empty, Title = string.Empty });
            if (scheduleId.HasValue)
            {
                using (var logic = new tblScheduleLogic())
                {
                    var scheduleItem = logic.GetItem(scheduleId.Value);
                    if (scheduleItem != null)
                    {
                        scheduleLine = scheduleItem.Line;
                    }
                }
            }
            var lineIdRes = scheduleLine != null ? scheduleLine.Id : lineId ?? 0;
            using (var logic = new LineLogic())
            {
                buses.AddRange(logic.GetAvailableBuses(lineIdRes)
                    .Select(z => new SelectItemModel
                    {
                        Value = z.Id.ToString(),
                        Text = string.Format("{0} ({1} - {2})", z.Id, z.BusId, z.PlateNumber),
                        Title = string.Format("{0} ({1} - {2} - {3} - {4})", z.Id, z.BusId, z.PlateNumber, z.BusCompany != null ? z.BusCompany.companyName : string.Empty, z.seats.HasValue ? z.seats.Value.ToString() : string.Empty),
                        Selected = z.BusesToLines.Any() && z.BusesToLines.First().LineId == lineIdRes
                    }).ToList());
            }

            return new JsonResult { Data = buses };
        }

        private List<DateTime> GetScheduleLineDates(Line line, DateTime dateFrom, DateTime dateTo, ScheduleParamsModel parameters)
        {
            var dates = new List<DateTime>();
            for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
            {
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (line.Sun.HasValue && line.Sun.Value && parameters.Sun) { dates.Add(date); }
                        break;
                    case DayOfWeek.Monday:
                        if (line.Mon.HasValue && line.Mon.Value && parameters.Mon) { dates.Add(date); }
                        break;
                    case DayOfWeek.Tuesday:
                        if (line.Tue.HasValue && line.Tue.Value && parameters.Tue) { dates.Add(date); }
                        break;
                    case DayOfWeek.Wednesday:
                        if (line.Wed.HasValue && line.Wed.Value && parameters.Wed) { dates.Add(date); }
                        break;
                    case DayOfWeek.Thursday:
                        if (line.Thu.HasValue && line.Thu.Value && parameters.Thu) { dates.Add(date); }
                        break;
                    case DayOfWeek.Friday:
                        if (line.Fri.HasValue && line.Fri.Value && parameters.Fri) { dates.Add(date); }
                        break;
                    case DayOfWeek.Saturday:
                        if (line.Sut.HasValue && line.Sut.Value && parameters.Sut) { dates.Add(date); }
                        break;
                }
            }
            return dates;
        }

        //public HttpResponseMessage GetExcel(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        //{
        //    var lines = new GridLineModel[] {};
        //    var totalRecords = 0;
        //    using (var logic = new LineLogic())
        //    {
        //        totalRecords = logic.Lines.Count();
        //        lines = logic.GetPaged(_search, totalRecords, 1, sidx, sord, filters)
        //            .Select(z => new GridLineModel(z)).ToArray();
        //    }

        //    string Name = "Lines";
        //    var workbook = new XLWorkbook();
        //    var worksheet = workbook.Worksheets.Add(Name + " Sheet");
        //    worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

        //    worksheet.Cell(1, 1).Value = DictExpressionBuilderSystem.Translate("Line.LineName");
        //    worksheet.Cell(1, 2).Value = DictExpressionBuilderSystem.Translate("Line.LineNumber");
        //    worksheet.Cell(1, 3).Value = DictExpressionBuilderSystem.Translate("Line.Direction");
        //    worksheet.Cell(1, 4).Value = DictExpressionBuilderSystem.Translate("Line.IsActive");
        //    worksheet.Cell(1, 5).Value = DictExpressionBuilderSystem.Translate("Line.totalStudents");
        //    worksheet.Cell(1, 6).Value = DictExpressionBuilderSystem.Translate("Line.Duration");
        //    worksheet.Cell(1, 7).Value = DictExpressionBuilderSystem.Translate("Line.Sun");
        //    worksheet.Cell(1, 8).Value = DictExpressionBuilderSystem.Translate("Line.Mon");
        //    worksheet.Cell(1, 9).Value = DictExpressionBuilderSystem.Translate("Line.Tue");
        //    worksheet.Cell(1, 10).Value = DictExpressionBuilderSystem.Translate("Line.Wed");
        //    worksheet.Cell(1, 11).Value = DictExpressionBuilderSystem.Translate("Line.Thu");
        //    worksheet.Cell(1, 12).Value = DictExpressionBuilderSystem.Translate("Line.Fri");
        //    worksheet.Cell(1, 13).Value = DictExpressionBuilderSystem.Translate("Line.Sut");
        //    worksheet.Cell(1, 14).Value = DictExpressionBuilderSystem.Translate("Bus.BusId");
        //    worksheet.Cell(1, 15).Value = DictExpressionBuilderSystem.Translate("Bus.PlateNumber");
        //    worksheet.Cell(1, 16).Value = DictExpressionBuilderSystem.Translate("BusCompany.Name");
        //    worksheet.Cell(1, 17).Value = DictExpressionBuilderSystem.Translate("Bus.seats");

        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        var row = 2 + i;
        //        worksheet.Cell(row, 1).SetValue<string>(lines[i].LineName);
        //        worksheet.Cell(row, 2).SetValue<string>(lines[i].LineNumber);
        //        worksheet.Cell(row, 3).SetValue<string>(lines[i].Direction == 0? "To": "From");
        //        worksheet.Cell(row, 4).SetValue<bool>(lines[i].IsActive);
        //        worksheet.Cell(row, 5).SetValue<int>(lines[i].totalStudents);
        //        worksheet.Cell(row, 6).SetValue<string>(Convert.ToString(lines[i].Duration));
        //        worksheet.Cell(row, 7).SetValue<bool>(lines[i].Sun);
        //        worksheet.Cell(row, 8).SetValue<bool>(lines[i].Mon);
        //        worksheet.Cell(row, 9).SetValue<bool>(lines[i].Tue);
        //        worksheet.Cell(row, 10).SetValue<bool>(lines[i].Wed);
        //        worksheet.Cell(row, 11).SetValue<bool>(lines[i].Thu);
        //        worksheet.Cell(row, 12).SetValue<bool>(lines[i].Fri);
        //        worksheet.Cell(row, 13).SetValue<bool>(lines[i].Sut);
        //        worksheet.Cell(row, 14).SetValue<string>(lines[i].BusIdDescription);
        //        worksheet.Cell(row, 15).SetValue<string>(lines[i].PlateNumber);
        //        worksheet.Cell(row, 16).SetValue<string>(lines[i].BusCompanyName);
        //        worksheet.Cell(row, 17).SetValue<int?>(lines[i].seats);
        //    }

        //    worksheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        //    worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.None;
        //    worksheet.Columns().AdjustToContents();

        //    var result = new HttpResponseMessage(HttpStatusCode.OK);
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        workbook.SaveAs(memoryStream);
        //        result.Content = new ByteArrayContent(memoryStream.GetBuffer());
        //        result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = Name + ".xlsx"
        //        };
        //        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //    }
        //    return result;
        //}


        //public GridLineModel[] GetPrint(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        //{
        //    var lines = new GridLineModel[] { };
        //    var totalRecords = 0;
        //    using (var logic = new LineLogic())
        //    {
        //        totalRecords = logic.Lines.Count();
        //        lines = logic.GetPaged(_search, totalRecords, page, sidx, sord, filters)
        //            .Select(z => new GridLineModel(z)).ToArray();
        //    }

        //    return lines;
        //}
    }
}