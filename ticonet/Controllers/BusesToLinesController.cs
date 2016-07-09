﻿using System;
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

namespace ticonet.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BusesToLinesController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LinesController));

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetLines(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var lines = new List<GridLineBusModel>();
            var totalRecords = 0;
            using (var logic = new LineLogic())
            {
                lines = logic.GetPaged(_search, rows, page, sidx, sord, filters)
                    .Select(z => new GridLineBusModel(z)).ToList();
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
        public JsonResult EditLine(GridLineBusModel model)
        {
            using (var logic = new LineLogic())
            {
                switch ((GridOperation)Enum.Parse(typeof(GridOperation), model.Oper, true))
                {
                    //case GridOperation.add:
                    //    logic.SaveLine(model.ToDbModel());
                    //    break;
                    case GridOperation.edit:
                        var existingLine = logic.GetLine(model.Id);
                        if (existingLine != null)
                        {
                            //model.UpdateDbModel(existingLine);
                            //logic.SaveChanges();
                            logic.UpdateBusToLine(model.Id, model.BusId);
                        }
                        break;
                    case GridOperation.del:
                        logic.DeleteLine(model.Id);
                        break;
                }
            }
            return new JsonResult {Data = true};
        }

        public HttpResponseMessage GetExcel(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var lines = new GridLineBusModel[] {};
            var totalRecords = 0;
            using (var logic = new LineLogic())
            {
                totalRecords = logic.Lines.Count();
                lines = logic.GetPaged(_search, totalRecords, 1, sidx, sord, filters)
                    .Select(z => new GridLineBusModel(z)).ToArray();
            }

            string Name = "BusesToLines";
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add($"{Name} Sheet");
            worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            worksheet.Cell(1, 1).Value = DictExpressionBuilderSystem.Translate("Line.LineName");
            worksheet.Cell(1, 2).Value = DictExpressionBuilderSystem.Translate("Line.LineNumber");
            worksheet.Cell(1, 3).Value = DictExpressionBuilderSystem.Translate("Line.Direction");
            worksheet.Cell(1, 4).Value = DictExpressionBuilderSystem.Translate("Line.IsActive");
            worksheet.Cell(1, 5).Value = DictExpressionBuilderSystem.Translate("Line.totalStudents");
            worksheet.Cell(1, 6).Value = DictExpressionBuilderSystem.Translate("Line.Duration");
            worksheet.Cell(1, 7).Value = DictExpressionBuilderSystem.Translate("Bus.BusId");
            worksheet.Cell(1, 8).Value = DictExpressionBuilderSystem.Translate("Bus.PlateNumber");
            worksheet.Cell(1, 9).Value = DictExpressionBuilderSystem.Translate("BusCompany.Name");
            worksheet.Cell(1, 10).Value = DictExpressionBuilderSystem.Translate("Bus.seats");

            for (int i = 0; i < lines.Length; i++)
            {
                var row = 2 + i;
                worksheet.Cell(row, 1).SetValue<string>(lines[i].LineName);
                worksheet.Cell(row, 2).SetValue<string>(lines[i].LineNumber);
                worksheet.Cell(row, 3).SetValue<string>(lines[i].Direction == 0? "To": "From");
                worksheet.Cell(row, 4).SetValue<bool>(lines[i].IsActive);
                worksheet.Cell(row, 5).SetValue<int>(lines[i].totalStudents);
                worksheet.Cell(row, 6).SetValue<string>(Convert.ToString(lines[i].Duration));
                worksheet.Cell(row, 7).SetValue<string>(lines[i].BusIdDescription);
                worksheet.Cell(row, 8).SetValue<string>(lines[i].PlateNumber);
                worksheet.Cell(row, 9).SetValue<string>(lines[i].BusCompanyName);
                worksheet.Cell(row, 10).SetValue<int?>(lines[i].seats);
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


        public GridLineBusModel[] GetPrint(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var lines = new GridLineBusModel[] { };
            var totalRecords = 0;
            using (var logic = new LineLogic())
            {
                totalRecords = logic.Lines.Count();
                lines = logic.GetPaged(_search, totalRecords, 1, sidx, sord, filters)
                    .Select(z => new GridLineBusModel(z)).ToArray();
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
                        Title = string.Format("{0} ({1} - {2} - {3})", z.BusId, z.PlateNumber, z.BusCompany!=null?z.BusCompany.companyName:string.Empty, z.seats.HasValue? z.seats.Value.ToString(): string.Empty)
                    }).ToList());
            }

            return new JsonResult { Data = buses };
        }

        public JsonResult GetCompaniesFilter()
        {
            var companies = new List<SelectItemModel>();
            companies.Add(new SelectItemModel { Value = "", Text = DictExpressionBuilderSystem.Translate("Search.All")});
            using (var logic = new LineLogic())
            {
                var getCompanies = logic.GetCompaniesFilter();
                companies.AddRange(getCompanies
                    .Where(w=> w != null)
                    .Select(z => new SelectItemModel
                    {
                        Value = z.pk.ToString(),
                        Text = z.companyName
                    }).ToList());
            }

            return new JsonResult { Data = companies };
        }
    }
}