using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Business_Logic;
using Business_Logic.Entities;
using log4net;
using ticonet.Models;
using Business_Logic.Enums;
using Business_Logic.Helpers;
using Business_Logic.Services;

namespace ticonet.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ScheduleApiController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ScheduleApiController));

        private IScheduleService scheduleService { get; set; }

        public ScheduleApiController()
        {
            scheduleService = new ScheduleService();
        }

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
            //var dateFrom = (DateTime?)DateHelper.GetSunday(DateTime.Now); 
            //var dateTo = (DateTime?)dateFrom.Value.AddDays(7).Date;
            //parameters.DateFrom = DateHelper.DateToString(dateFrom);
            //parameters.DateTo = DateHelper.DateToString(dateTo);

            var dateFrom = DateHelper.StringToDate(parameters.DateFrom);
            var dateTo = DateHelper.StringToDate(parameters.DateTo);
            var fakeId = 0;

            if (!dateFrom.HasValue || !dateTo.HasValue || string.IsNullOrEmpty(parameters.LinesIds))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var schedule = scheduleService.GenerateSchedule(parameters)
                .Select(x => new ScheduleItemModel(x))
                .ToList();
            
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    rows = schedule
                });
        }

        [System.Web.Http.HttpPost]
        public JsonResult SaveGeneratedShcedule(IEnumerable<ScheduleItemModel> model, string dateFrom, string dateTo)
        {
            //var dtDateFrom = (DateTime?)DateHelper.GetSunday(DateTime.Now);
            //var dtDateTo = (DateTime?)dtDateFrom.Value.AddDays(7).Date;

            var dtDateFrom = DateHelper.StringToDate(dateFrom);
            var dtDateTo = DateHelper.StringToDate(dateTo);
            var result = false;
            var items = model != null ? model.ToArray() : new ScheduleItemModel[0];

            if (items.Any() && dtDateFrom.HasValue && dtDateTo.HasValue)
            {
                foreach (var item in items)
                {
                    item.LineId = item.LineIdKey;
                    item.DriverId = item.DriverIdKey;
                    item.BusId = item.BusIdKey;
                }
                result = scheduleService.SaveGeneratedShcedule(items.Select(x => x.ToDbModel()), dtDateFrom.Value, dtDateTo.Value);
            }
            return new JsonResult { Data = result };
        }

        [System.Web.Http.HttpGet]
        public async Task<HttpResponseMessage> FillSchedule()
        {
            var success = await Task<bool>.Factory.StartNew(() =>
            {
                return scheduleService.PopulateLinesPlan();
            });

            return Request.CreateResponse(!success ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
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

    }
}