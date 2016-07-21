using Business_Logic;
using Business_Logic.Dtos;
using Business_Logic.Enums;
using ClosedXML.Excel;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using DEBS = Business_Logic.DictExpressionBuilderSystem;

namespace ticonet.Helpers {

    public static class ExcellWriter {

        public static XLWorkbook AllLinesPeriodStatistic(List<LinePeriodStatisticDto> data) {
            var dates = data[0].DayDate;

            var book = new XLWorkbook();
            var sheet = book.Worksheets.Add("Sheet");
            sheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            //Table Header
            int rowIndex = 2;
            sheet.Cell(rowIndex, 1).Value = DEBS.Translate("Line.Report");
            rowIndex += 2;
            sheet.Cell(rowIndex, 1).Value = DEBS.Translate("Line.LineNumber");

            for (int i = 0; i < dates.Count; i++)
                sheet.Cell(rowIndex, 2+i).Value = dates[i].ToString(@"ddd dd/MM/yyyy");
            rowIndex++;
            //Content
            for (int i = 0; i < data.Count; i++) {
                //Pure Row
                sheet.Cell(rowIndex, 1).Value = data[i].LineNumber;
                for (int colomn = 0; colomn < dates.Count; colomn++) {
                    sheet.Cell(rowIndex, 2 +colomn ).Value = data[i].DayIsScheduled[colomn] ? "+":"-";
                }
                rowIndex++;
                //Info
                sheet.Cell(rowIndex, 1).Value = DEBS.Translate("Line.Name");
                sheet.Cell(rowIndex, 2).Value = data[i].LineName;
                rowIndex++;
                sheet.Cell(rowIndex, 1).Value = DEBS.Translate("Line.Direction");
                sheet.Cell(rowIndex, 2).Value = DirectionToString(data[i].Direction);
                rowIndex++;
                sheet.Cell(rowIndex, 1).Value = DEBS.Translate("Line.totalStudents");
                sheet.Cell(rowIndex, 2).Value = data[i].totalStudents;
                rowIndex++;
                sheet.Cell(rowIndex, 1).Value = DEBS.Translate("Bus.CompanyName");
                sheet.Cell(rowIndex, 2).Value = data[i].BusCompanyName;
                rowIndex++;
                sheet.Cell(rowIndex, 1).Value = DEBS.Translate("Bus.seats");
                sheet.Cell(rowIndex, 2).Value = data[i].seats;
                rowIndex++;
                sheet.Cell(rowIndex, 1).Value = DEBS.Translate("Bus.price");
                sheet.Cell(rowIndex, 2).Value = data[i].price;
                rowIndex++;
            }

            sheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            sheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.None;
            sheet.Columns().AdjustToContents();

            return book;
        }

        public static XLWorkbook AllLinesTotalStatistic(List<LinesTotalStatisticDto> data) {
            var book = new XLWorkbook();
            var sheet = book.Worksheets.Add("Sheet");
            sheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            //Table Header
            int rowIndex = 2;
            sheet.Cell(rowIndex, 1).Value = DEBS.Translate("Line.SummaryReport");
            rowIndex += 2;
            int colIndex = 1;
            sheet.Cell(rowIndex, colIndex).Value = DEBS.Translate("Report.Month");
            sheet.Cell(rowIndex + 1, colIndex).Value = DEBS.Translate("Report.linesCount");
            sheet.Cell(rowIndex + 2, colIndex).Value = DEBS.Translate("Report.totalStudents");
            sheet.Cell(rowIndex + 3, colIndex).Value = DEBS.Translate("Report.totalPrice");
            colIndex++;
            for (int i=0; i < data.Count; i++) {
                sheet.Cell(rowIndex, colIndex+i).Value = i+1;
                sheet.Cell(rowIndex + 1, colIndex + i).Value = data[i].linesCount;
                sheet.Cell(rowIndex + 2, colIndex + i).Value = data[i].totalStudents;
                sheet.Cell(rowIndex + 3, colIndex + i).Value = data[i].totalPrice;
            }

            sheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            sheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.None;
            sheet.Columns().AdjustToContents();

            return book;
        }

        public static HttpResponseMessage BookToHTTPResponseMsg (XLWorkbook book, string fileName) {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            using (var memoryStream = new MemoryStream()) {
                book.SaveAs(memoryStream);
                result.Content = new ByteArrayContent(memoryStream.GetBuffer());
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") {
                    FileName = fileName + ".xlsx"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            return result;
        }

        static string DirectionToString(int dirValue) {
            return dirValue == ((int)LineDirection.Bouth) ?
                DEBS.Translate("General.Bouth") :
                dirValue == ((int)LineDirection.To) ?
                DEBS.Translate("General.To") :
                DEBS.Translate("General.From");
        }

    }
}