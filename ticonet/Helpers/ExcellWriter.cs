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

        public static XLWorkbook NewBook () {
            return new XLWorkbook();
        }

        public static IXLWorksheet NewReportSheet (XLWorkbook book, string SheetName, string Header, int HeaderRow) {
            var sh = book.Worksheets.Add(SheetName);
            var cols = sh.Columns();
            cols.Width = 18;
            cols.Style.Fill.BackgroundColor = XLColor.FromArgb(34,34,34);
            cols.Style.Font.FontColor = XLColor.LightGray;
            sh.Cell(HeaderRow, 1).Value = Header;
            RowStyle_H1(sh.Row(HeaderRow));
            sh.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;
            return sh;
        }

        static void RowStyle_H1 (IXLRow row) {
            row.Height = 30;
            row.Style.Font.FontSize = 20;
        }
        static void RowStyle_H2 (IXLRow row) {
            row.Height = 20;
            row.Style.Font.FontSize = 16;
        }

        static void FoldRows (IXLRows rows) {
            rows.Style.Fill.BackgroundColor = XLColor.FromArgb(45, 45, 45);
            IXLRow last = null;
            rows.ForEach(x => last = x);
            last.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            last.Style.Border.BottomBorderColor = XLColor.DarkGray;
            rows.Group(1, true);
        }

        static void MakeBottomBorder (IXLRow row) {
            row.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            row.Style.Border.BottomBorderColor = XLColor.DarkGray;
        }
        static void MakeFooterStyle(IXLRow row) {
            row.Style.Fill.BackgroundColor = XLColor.SeaGreen;
        }

        ///<summary>
        ///returns next spare row number
        ///</summary>
        public static int AddLinesPeriodStatisticToSheet (IXLWorksheet toSheet, int startRow, List<LinePeriodStatisticDto> data, List<LinesDatedTotalStatisticDto> footerData) {
            var dates = data[0].DayDate;

            toSheet.Cell(startRow, 1).Value = DEBS.Translate("Line.ReportByDays");
            RowStyle_H2(toSheet.Row(startRow));
            startRow += 1;
            toSheet.Cell(startRow, 1).Value = DEBS.Translate("Line.LineNumber");

            for (int i = 0; i < dates.Count; i++)
                toSheet.Cell(startRow, 2 + i).Value = dates[i].ToString(@"ddd dd/MM/yyyy");
            startRow++;
            for (int i = 0; i < data.Count; i++) {
                //Pure Row
                toSheet.Cell(startRow, 1).Value = data[i].LineNumber;
                toSheet.Row(startRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                MakeBottomBorder(toSheet.Row(startRow));
                for (int colomn = 0; colomn < dates.Count; colomn++) {
                    var c = toSheet.Cell(startRow, 2 + colomn);
                    bool scheduled = data[i].DayScheduleData[colomn] != LinePeriodStatisticDto.DayScheduleData_INACTIVE;
                    c.Value = scheduled ? data[i].DayScheduleData[colomn] : "-";
                    if (scheduled)
                        c.Style.Fill.BackgroundColor = XLColor.SeaGreen;
                }
                startRow++;
                //Info
                var infoRowStart = startRow;
                toSheet.Cell(startRow, 2).Value = DEBS.Translate("Line.Name");
                toSheet.Cell(startRow, 3).Value = data[i].LineName;
                startRow++;
                toSheet.Cell(startRow, 2).Value = DEBS.Translate("Line.Direction");
                toSheet.Cell(startRow, 3).Value = DirectionToString(data[i].Direction);
                startRow++;
                toSheet.Cell(startRow, 2).Value = DEBS.Translate("Line.totalStudents");
                toSheet.Cell(startRow, 3).Value = data[i].totalStudents;
                startRow++;
                toSheet.Cell(startRow, 2).Value = DEBS.Translate("Bus.CompanyName");
                toSheet.Cell(startRow, 3).Value = data[i].BusCompanyName;
                startRow++;
                toSheet.Cell(startRow, 2).Value = DEBS.Translate("Bus.seats");
                toSheet.Cell(startRow, 3).Value = data[i].seats;
                startRow++;
                toSheet.Cell(startRow, 2).Value = DEBS.Translate("Bus.price");
                toSheet.Cell(startRow, 3).Value = data[i].price;
                //styling and collapsing
                FoldRows(toSheet.Rows(infoRowStart, startRow));
                startRow++;
            }
            toSheet.CollapseRows(1);
            //Footer
            toSheet.Cell(startRow, 1).Value = DEBS.Translate("Lines.totalPrice");
            toSheet.Row(startRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            MakeFooterStyle(toSheet.Row(startRow));
            for (int colomn = 0; colomn < dates.Count; colomn++) {
                var c = toSheet.Cell(startRow, 2 + colomn);
                c.Value = footerData[colomn].totalPrice;
            }
            startRow++;
            return startRow;
        }

        ///<summary>
        ///returns next spare row number
        ///</summary>
        public static int AddLinesSummaryStatisticToSheet (IXLWorksheet toSheet, int startRow, List<LinesTotalStatisticDto> data) {
            //Table Header
            toSheet.Cell(startRow, 1).Value = DEBS.Translate("Line.SummaryReport");
            RowStyle_H2(toSheet.Row(startRow));
            startRow++;
            int colIndex = 1;
            toSheet.Cell(startRow, colIndex).Value = DEBS.Translate("Report.Month");
            toSheet.Cell(startRow + 1, colIndex).Value = DEBS.Translate("Report.linesCount");
            toSheet.Cell(startRow + 2, colIndex).Value = DEBS.Translate("Report.totalStudents");
            toSheet.Cell(startRow + 3, colIndex).Value = DEBS.Translate("Report.totalPrice");
            colIndex++;
            for (int i = 0; i < data.Count; i++) {
                toSheet.Cell(startRow, colIndex + i).Value = i + 1;
                toSheet.Cell(startRow + 1, colIndex + i).Value = data[i].linesCount;
                toSheet.Cell(startRow + 2, colIndex + i).Value = data[i].totalStudents;
                toSheet.Cell(startRow + 3, colIndex + i).Value = data[i].totalPrice;
            }
            return startRow +4;
        }

        public static HttpResponseMessage BookToHTTPResponseMsg (XLWorkbook book, string fileName) {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            using (var memoryStream = new MemoryStream()) {
                book.SaveAs(memoryStream);
                memoryStream.Position = 0;
                result.Content = new ByteArrayContent(memoryStream.ToArray());
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