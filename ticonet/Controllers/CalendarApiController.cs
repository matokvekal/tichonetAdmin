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
using Business_Logic.Enums;
using ClosedXML.Excel;
using log4net;
using ticonet.Models;

namespace ticonet.Controllers
{
    [System.Web.Mvc.Authorize]
    public class CalendarApiController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CalendarApiController));

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetEvents(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var events = new List<EventModel>();
            var totalRecords = 0;
            using (var logic = new tblCalendarLogic())
            {
                events = logic.GetPaged(_search, rows, page, sidx, sord, filters)
                    .Select(z => new EventModel(z)).ToList();
                totalRecords = logic.Events.Count();
            }
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    total = (totalRecords + rows - 1) / rows,
                    page,
                    records = totalRecords,
                    rows = events
                });
        }
        
        [System.Web.Mvc.HttpPost]
        public JsonResult EditEvent(EventModel model)
        {
            using (var logic = new tblCalendarLogic())
            {
                switch ((GridOperation)Enum.Parse(typeof(GridOperation), model.Oper, true))
                {
                    //case GridOperation.add:
                    //    logic.SaveBus(model.ToDbModel());
                    //    break;
                    case GridOperation.edit:
                        var @event = logic.GetEvent(model.pk);
                        if (@event != null)
                        {
                            @event.active = model.active;
                            @event.@event = model.calendarEvent;
                            logic.Update(@event);
                        }
                        break;
                    //case GridOperation.del:
                    //    logic.DeleteBus(model.Id);
                    //    break;
                }
            }
            return new JsonResult {Data = true};
        }

        public HttpResponseMessage GetExcel(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var events = new EventModel[] {};
            var totalRecords = 0;
            using (var logic = new tblCalendarLogic())
            {
                totalRecords = logic.Events.Count();
                events = logic.GetPaged(_search, totalRecords, 1, sidx, sord, filters)
                    .Select(z => new EventModel(z)).ToArray();
            }

            string Name = "Calendar";
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(Name + " Sheet");
            worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            worksheet.Cell(1, 1).Value = DictExpressionBuilderSystem.Translate("Event.date");
            worksheet.Cell(1, 2).Value = DictExpressionBuilderSystem.Translate("Event.month");
            worksheet.Cell(1, 3).Value = DictExpressionBuilderSystem.Translate("Event.HebMonth");
            worksheet.Cell(1, 4).Value = DictExpressionBuilderSystem.Translate("Event.day");
            worksheet.Cell(1, 5).Value = DictExpressionBuilderSystem.Translate("Event.active");
            worksheet.Cell(1, 6).Value = DictExpressionBuilderSystem.Translate("Event.event");

            for (int i = 0; i < events.Length; i++)
            {
                var row = 2 + i;
                worksheet.Cell(row, 1).SetValue<string>(events[i].date);
                worksheet.Cell(row, 2).SetValue<string>(events[i].month);
                worksheet.Cell(row, 3).SetValue<string>(events[i].HebMonth);
                worksheet.Cell(row, 4).SetValue<string>(events[i].day);
                worksheet.Cell(row, 5).SetValue<bool?>(events[i].active);
                worksheet.Cell(row, 6).SetValue<string>(events[i].calendarEvent);
            }

            worksheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.None;
            worksheet.Columns().AdjustToContents();

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                result.Content = new ByteArrayContent(memoryStream.ToArray());
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = Name + ".xlsx"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            return result;
        }


        public EventModel[] GetPrint(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var events = new EventModel[] { };
            var totalRecords = 0;
            using (var logic = new tblCalendarLogic())
            {
                totalRecords = logic.Events.Count();
                events = logic.GetPaged(_search, totalRecords, page, sidx, sord, filters)
                    .Select(z => new EventModel(z)).ToArray();
            }

            return events;
        }

        [System.Web.Mvc.HttpGet]
        public int[] GetDaysWithEvents(int month, int year)
        {
            using (var logic = new tblCalendarLogic())
            {
                return logic.GetDaysWithEvents(month, year);
            }
        }
    }
}