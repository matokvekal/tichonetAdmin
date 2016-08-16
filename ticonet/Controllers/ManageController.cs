using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml.Linq;
using Business_Logic;
using Business_Logic.Entities;
using ClosedXML.Excel;
using log4net;
using MvcJqGrid;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using DEBS = Business_Logic.DictExpressionBuilderSystem;
using System.Drawing;


namespace ticonet
{
    public class ManageController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ManageController));

        public ActionResult Index()
        {
            ViewBag.totalStudents = tblStudentLogic.totalStudents();
            ViewBag.totalFamilies = tblFamilyLogic.totalRegistrationStudentsFamilies();
            ViewBag.totalRegistrationStudents = tblStudentLogic.totalRegistrationStudents();

            return View();
        }

        public void btnExportToExcel_Click2(object sender, EventArgs e)
        {
            logger.Info("btnExportToExcel_Click2 ");
            List<ViewAllStudentFamilyLinesStation> c = ViewAllStudentsLogic.GetAllStudents();
            logger.Info("after list ");
            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));

            StringBuilder sb = new StringBuilder();
            sb.Append("<table>");

            //Compose header row
            sb.Append("<tr>");
            sb.Append("<th>" + DEBS.Translate("Report.totalPrice") + "</th>");
            sb.Append("</tr>");


            //   worksheet.Cell(row, "G").Value = stud.PayStatus == true ? "Yes" : "No";
            //foreach (tblMain row in list)
            //{
            sb.Append("<tr>");
            //sb.Append(String.Format("<td>{0}</td>", row.FirstName));
            //sb.Append(String.Format("<td>{0}</td>", row.LastName));
            //sb.Append(String.Format("<td>{0}</td>", row.Email1));
            sb.Append("</tr>");
            //}
            sb.Append("</table>");

            ExportToExcel(sb.ToString());
        }


        private void ExportToExcel(string content)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=Book1.xls");
            Response.Charset = "";
            //  this.EnableViewState = false;

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

            htw.WriteLine(content);

            Response.Write(sw.ToString());
            Response.End();
        }


        //[HttpPost]
        //[Authorize]
        public FileContentResult btnExportToExcel_Click()
        {

            LineLogic.updateAriveAndDepartureTime();//update table Lines- insert data to BasicArriveTime & BasicDepartureTime

            List<ViewAllStudentFamilyLinesStation> lst = ViewAllStudentsLogic.GetAllStudents();
            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));



            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Students");
            worksheet.Cell("A1").Value = DEBS.Translate("General.siduri");
            worksheet.Cell("B1").Value = DEBS.Translate("tblStudent.studentId");
            worksheet.Cell("C1").Value = DEBS.Translate("tblStudent.firstName");
            worksheet.Cell("D1").Value = DEBS.Translate("tblStudent.lastName");
            worksheet.Cell("E1").Value = DEBS.Translate("tblStudent.class");
            worksheet.Cell("F1").Value = DEBS.Translate("tblStudent.city");
            worksheet.Cell("G1").Value = DEBS.Translate("tblStudent.street");
            worksheet.Cell("H1").Value = DEBS.Translate("tblStudent.houseNumber");
            worksheet.Cell("I1").Value = DEBS.Translate("tblStudent.CellPhone");
            worksheet.Cell("J1").Value = DEBS.Translate("tblStudent.Email");
            worksheet.Cell("K1").Value = DEBS.Translate("tblStudent.registrationStatus");
            worksheet.Cell("L1").Value = DEBS.Translate("tblFamily.subsidy");
            worksheet.Cell("M1").Value = DEBS.Translate("tblStudent.Active");
            worksheet.Cell("N1").Value = DEBS.Translate("tblStudent.siblingAtSchool");
            worksheet.Cell("O1").Value = DEBS.Translate("excel.parent1FirstName");
            worksheet.Cell("P1").Value = DEBS.Translate("excel.parent1LastName");
            worksheet.Cell("Q1").Value = DEBS.Translate("excel.parent1Email");
            worksheet.Cell("R1").Value = DEBS.Translate("excel.parent1CellPhone");
            worksheet.Cell("S1").Value = DEBS.Translate("excel.parent2FirstName");
            worksheet.Cell("T1").Value = DEBS.Translate("excel.parent2Email");
            worksheet.Cell("U1").Value = DEBS.Translate("excel.parent2CellPhone");
            worksheet.Cell("V1").Value = DEBS.Translate("tblFamily.subsidy");
            worksheet.Cell("W1").Value = DEBS.Translate("tblFamily.registrationStatus");
            worksheet.Cell("X1").Value = DEBS.Translate("studentToLoines.distanceFromStation");
            worksheet.Cell("Y1").Value = DEBS.Translate("Line.LineName");
            worksheet.Cell("Z1").Value = DEBS.Translate("Line.LineNumber");
            worksheet.Cell("AA1").Value = DEBS.Translate("tblStudent.school");
            worksheet.Cell("AB1").Value = DEBS.Translate("Line.Duration");
            worksheet.Cell("AC1").Value = DEBS.Translate("Stations.StationName");
            worksheet.Cell("AD1").Value = DEBS.Translate("StationsToLines.ArrivalDate");
            worksheet.Cell("AE1").Value = DEBS.Translate("StationsToLines.ArrivalTimeSun");
            worksheet.Cell("AF1").Value = DEBS.Translate("StationsToLines.ArrivalTimeMon");
            worksheet.Cell("AG1").Value = DEBS.Translate("StationsToLines.ArrivalTimeTue");
            worksheet.Cell("AH1").Value = DEBS.Translate("StationsToLines.ArrivalTimeWed");
            worksheet.Cell("AI1").Value = DEBS.Translate("StationsToLines.ArrivalTimeThu");
            worksheet.Cell("AJ1").Value = DEBS.Translate("StationsToLines.ArrivalTimeFri");
            worksheet.Cell("AK1").Value = DEBS.Translate("StationsToLines.ArrivalTimeSat");


            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("A").Width = 6;
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("B").Width = 12;
            worksheet.Cell("C1").Style.Font.Bold = true;
            worksheet.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("C").Width = 12;
            worksheet.Cell("D1").Style.Font.Bold = true;
            worksheet.Cell("D1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("D").Width = 8;
            worksheet.Cell("E1").Style.Font.Bold = true;
            worksheet.Cell("E1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("E").Width = 8;
            worksheet.Cell("F1").Style.Font.Bold = true;
            worksheet.Cell("F1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("F").Width = 20;
            worksheet.Cell("G1").Style.Font.Bold = true;
            worksheet.Cell("G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("G").Width = 8;
            worksheet.Cell("H1").Style.Font.Bold = true;
            worksheet.Cell("H1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("H").Width = 8;
            worksheet.Cell("I1").Style.Font.Bold = true;
            worksheet.Cell("I1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("I").Width = 8;
            worksheet.Cell("J1").Style.Font.Bold = true;
            worksheet.Cell("J1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("J").Width = 14;
            worksheet.Cell("K1").Style.Font.Bold = true;
            worksheet.Cell("K1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("K").Width = 16;
            worksheet.Cell("L1").Style.Font.Bold = true;
            worksheet.Cell("L1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("L").Width = 20;

            worksheet.Cell("M1").Style.Font.Bold = true;
            worksheet.Cell("M1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("M").Width = 6;
            worksheet.Cell("N1").Style.Font.Bold = true;
            worksheet.Cell("N1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("N").Width = 12;
            worksheet.Cell("O1").Style.Font.Bold = true;
            worksheet.Cell("O1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("O").Width = 12;
            worksheet.Cell("P1").Style.Font.Bold = true;
            worksheet.Cell("P1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("P").Width = 8;
            worksheet.Cell("Q1").Style.Font.Bold = true;
            worksheet.Cell("Q1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("Q").Width = 8;
            worksheet.Cell("R1").Style.Font.Bold = true;
            worksheet.Cell("R1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("R").Width = 20;
            worksheet.Cell("S1").Style.Font.Bold = true;
            worksheet.Cell("S1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("S").Width = 8;
            worksheet.Cell("T1").Style.Font.Bold = true;
            worksheet.Cell("T1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("T").Width = 8;
            worksheet.Cell("U1").Style.Font.Bold = true;
            worksheet.Cell("U1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("U").Width = 8;
            worksheet.Cell("V1").Style.Font.Bold = true;
            worksheet.Cell("V1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("V").Width = 14;
            worksheet.Cell("W1").Style.Font.Bold = true;
            worksheet.Cell("W1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("W").Width = 16;
            worksheet.Cell("X1").Style.Font.Bold = true;
            worksheet.Cell("X1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("X").Width = 20;

            worksheet.Cell("Y1").Style.Font.Bold = true;
            worksheet.Cell("Y1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("Y").Width = 6;
            worksheet.Cell("Z1").Style.Font.Bold = true;
            worksheet.Cell("Z1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("Z").Width = 12;
            worksheet.Cell("AA1").Style.Font.Bold = true;
            worksheet.Cell("AA1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AA").Width = 12;
            worksheet.Cell("AB1").Style.Font.Bold = true;
            worksheet.Cell("AB1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AB").Width = 8;
            worksheet.Cell("AC1").Style.Font.Bold = true;
            worksheet.Cell("AC1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AC").Width = 8;
            worksheet.Cell("AD1").Style.Font.Bold = true;
            worksheet.Cell("AD1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AD").Width = 8;
            worksheet.Cell("AE1").Style.Font.Bold = true;
            worksheet.Cell("AE1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AE").Width = 8;
            worksheet.Cell("AF1").Style.Font.Bold = true;
            worksheet.Cell("AF1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AF").Width = 8;
            worksheet.Cell("AG1").Style.Font.Bold = true;
            worksheet.Cell("AG1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AG").Width = 8;
            worksheet.Cell("AH1").Style.Font.Bold = true;
            worksheet.Cell("AH1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AH").Width = 8;
            worksheet.Cell("AI1").Style.Font.Bold = true;
            worksheet.Cell("AI1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AI").Width = 8;
            worksheet.Cell("AJ1").Style.Font.Bold = true;
            worksheet.Cell("AJ1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AJ").Width = 8;
            worksheet.Cell("AK1").Style.Font.Bold = true;
            worksheet.Cell("AK1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AK").Width = 8;


            var row = 2;
            foreach (var stud in lst)
            {

                worksheet.Cell(row, "A").Value = row - 1;
                worksheet.Cell(row, "B").Value = stud.studentId;
                worksheet.Cell(row, "C").Value = stud.studentFirstName;
                worksheet.Cell(row, "D").Value = stud.studentLastName;
                worksheet.Cell(row, "E").Value = stud.@class;
                worksheet.Cell(row, "F").Value = stud.city;
                worksheet.Cell(row, "G").Value = stud.street;
                worksheet.Cell(row, "H").Value = stud.houseNumber;
                worksheet.Cell(row, "I").Value = "'" + stud.studentCellPhone;
                worksheet.Cell(row, "J").Value = stud.studentEmail;
                worksheet.Cell(row, "K").Value = stud.registrationStatus == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "L").Value = stud.subsidy == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "M").Value = stud.studentActive == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "N").Value = stud.siblingAtSchool == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "O").Value = stud.parent1FirstName;
                worksheet.Cell(row, "P").Value = stud.parent1LastName;
                worksheet.Cell(row, "Q").Value = stud.parent1Email;
                worksheet.Cell(row, "R").Value = "'" + stud.parent1CellPhone;
                worksheet.Cell(row, "S").Value = stud.parent2FirstName;
                worksheet.Cell(row, "T").Value = stud.parent2Email;
                worksheet.Cell(row, "U").Value = "'" + stud.parent2CellPhone;
                worksheet.Cell(row, "V").Value = stud.subsidy == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "W").Value = stud.registrationStatus == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "X").Value = stud.distanceFromStation;
                worksheet.Cell(row, "Y").Value = stud.LineName;
                worksheet.Cell(row, "Z").Value = stud.LineNumber;
                worksheet.Cell(row, "AA").Value = stud.schoolName;
                worksheet.Cell(row, "AB").Value = stud.Duration;
                worksheet.Cell(row, "AC").Value = stud.StationName;
                worksheet.Cell(row, "AD").Value = stud.ArrivalDate;//time for bus in student station
                //     calculate diferent times in each day for same line(if there are)
                if (stud.LineNumber != null || stud.Linedirection != null)//student is connect to line
                {
                    //  logger.Info("in time span " + stud.LineNumber + " " + stud.Linedirection);
                    TimeSpan basic = new TimeSpan();
                    TimeSpan ArriveDate = new TimeSpan();
                    if (stud.ArrivalDate != null)
                        ArriveDate = stud.ArrivalDate.Value;
                    if (stud.Linedirection.Value == 0)
                        if (stud.BasicArriveTime != null)
                        {
                            basic = stud.BasicArriveTime.Value.TimeOfDay;
                            logger.Info("basicA " + basic);
                        }
                        else
                        {
                            if (stud.BasicDepartureTime != null)
                                basic = stud.BasicDepartureTime.Value.TimeOfDay;
                        }
                    if (stud.BasicArriveTime != null || stud.BasicDepartureTime != null)
                    {
                        //string p = (ArriveDate + stud.SunTime.Value.TimeOfDay - basic).ToString();
                        //TimeSpan r = (ArriveDate + stud.MonTime.Value.TimeOfDay - basic);
                        //TimeSpan v = (ArriveDate + stud.TueTime.Value.TimeOfDay - basic);
                        worksheet.Cell(row, "AE").Value = (stud.SunTime != null) ? (ArriveDate + stud.SunTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AF").Value = (stud.MonTime != null) ? (ArriveDate + stud.MonTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AG").Value = (stud.TueTime != null) ? (ArriveDate + stud.TueTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AH").Value = (stud.WedTime != null) ? (ArriveDate + stud.WedTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AI").Value = (stud.ThuTime != null) ? (ArriveDate + stud.ThuTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AJ").Value = (stud.FriTime != null) ? (ArriveDate + stud.FriTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AK").Value = (stud.SutTime != null) ? (ArriveDate + stud.SutTime.Value.TimeOfDay - basic).ToString() : null;
                    }
                }

                row++;
            }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            var sr = new BinaryReader(ms);
            return File(sr.ReadBytes((int)ms.Length), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx"); ;

        }



        public FileContentResult btnExportToExcel2()
        {

            LineLogic.updateAriveAndDepartureTime();

            List<ViewAllStudentFamilyLinesStation> lst = ViewAllStudentsLogic.GetAllStudents();
            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));

    


            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Students");
            worksheet.Cell("A1").Value = DEBS.Translate("General.siduri");
            worksheet.Cell("B1").Value = DEBS.Translate("tblStudent.firstName");
            worksheet.Cell("C1").Value = DEBS.Translate("tblStudent.lastName");
            worksheet.Cell("D1").Value = DEBS.Translate("tblStudent.city");
            worksheet.Cell("E1").Value = DEBS.Translate("tblStudent.street");
            worksheet.Cell("F1").Value = DEBS.Translate("tblStudent.houseNumber");
            worksheet.Cell("G1").Value = DEBS.Translate("Line.LineName");
            worksheet.Cell("H1").Value = DEBS.Translate("Line.LineNumber");
            worksheet.Cell("I1").Value = DEBS.Translate("Line.Duration");
            worksheet.Cell("J1").Value = DEBS.Translate("Stations.StationName");
            worksheet.Cell("K1").Value = DEBS.Translate("StationsToLines.ArrivalDate");



            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("A").Width = 6;
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("B").Width = 12;
            worksheet.Cell("C1").Style.Font.Bold = true;
            worksheet.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("C").Width = 12;
            worksheet.Cell("D1").Style.Font.Bold = true;
            worksheet.Cell("D1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("D").Width = 8;
            worksheet.Cell("E1").Style.Font.Bold = true;
            worksheet.Cell("E1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("E").Width = 8;
            worksheet.Cell("F1").Style.Font.Bold = true;
            worksheet.Cell("F1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("F").Width = 20;
            worksheet.Cell("G1").Style.Font.Bold = true;
            worksheet.Cell("G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("G").Width = 8;
            worksheet.Cell("H1").Style.Font.Bold = true;
            worksheet.Cell("H1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("H").Width = 8;
            worksheet.Cell("I1").Style.Font.Bold = true;
            worksheet.Cell("I1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("I").Width = 8;
            worksheet.Cell("J1").Style.Font.Bold = true;
            worksheet.Cell("J1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("J").Width = 14;
            worksheet.Cell("K1").Style.Font.Bold = true;
            worksheet.Cell("K1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("K").Width = 16;

            string lineNumberPrev=null;
            var row = 2;
            lst = lst.OrderBy(c => c.LineNumber).ThenBy(c => c.Position).ToList();

            foreach (var stud in lst)
            {
                if (row > 2 && lineNumberPrev!=stud.LineNumber)
                     row++;

                worksheet.Cell(row, "A").Value = row - 1;
                worksheet.Cell(row, "B").Value = stud.studentFirstName;
                worksheet.Cell(row, "C").Value = stud.studentLastName;
                worksheet.Cell(row, "D").Value = stud.city;
                worksheet.Cell(row, "E").Value = stud.street;
                worksheet.Cell(row, "F").Value = stud.houseNumber;
                worksheet.Cell(row, "G").Value = stud.LineName;
                worksheet.Cell(row, "H").Value =lineNumberPrev= stud.LineNumber;
                worksheet.Cell(row, "I").Value = stud.Duration;

                //     calculate diferent times in each day for same line(if there are)
                //if (stud.LineNumber != null || stud.Linedirection != null)//student is connect to line
                //{
                //    TimeSpan basic = new TimeSpan();
                //    TimeSpan ArriveDate = new TimeSpan();
                //    if (stud.ArrivalDate != null)
                //        ArriveDate = stud.ArrivalDate.Value;
                //    if (stud.Linedirection.Value == 0)
                //        if (stud.BasicArriveTime != null)
                //        {
                //            basic = stud.BasicArriveTime.Value.TimeOfDay;
                //            logger.Info("basicA " + basic);
                //        }
                //        else
                //        {
                //            if (stud.BasicDepartureTime != null)
                //                basic = stud.BasicDepartureTime.Value.TimeOfDay;
                //        }
                //    if (stud.BasicArriveTime != null || stud.BasicDepartureTime != null)
                //    {

                //        worksheet.Cell(row, "AE").Value = (stud.SunTime != null) ? (ArriveDate + stud.SunTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AF").Value = (stud.MonTime != null) ? (ArriveDate + stud.MonTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AG").Value = (stud.TueTime != null) ? (ArriveDate + stud.TueTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AH").Value = (stud.WedTime != null) ? (ArriveDate + stud.WedTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AI").Value = (stud.ThuTime != null) ? (ArriveDate + stud.ThuTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AJ").Value = (stud.FriTime != null) ? (ArriveDate + stud.FriTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AK").Value = (stud.SutTime != null) ? (ArriveDate + stud.SutTime.Value.TimeOfDay - basic).ToString() : null;
                //    }
                //}

                row++;
            }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            var sr = new BinaryReader(ms);
            return File(sr.ReadBytes((int)ms.Length), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx"); ;

        }

    }
}





