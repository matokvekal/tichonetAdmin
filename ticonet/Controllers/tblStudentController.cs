using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml.Linq;
using Business_Logic;
using Business_Logic.Entities;
using ClosedXML.Excel;
using MvcJqGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace ticonet
{
    public class tblStudentController : Controller
    {

        //[HttpPost]
        //public ActionResult Check()
        //{
        //    return Json(new { success = true });
        //}
        [Authorize]
        public void regPay(string studentId, string familyId)
        {
            var result = new PaymentController().getPayment(studentId, familyId);

            //  queryMPITransaction(transaction_id);
        }



        //[Authorize]
        //public JsonResult regPay1(string h)
        //{
        //    return Json(JsonRequestBehavior.AllowGet);
        //}
        //------------------------------------//
        // GET: tblStudent
        public ActionResult Index()
        {
            using (var logic = new tblStudentLogic())
            {
                ViewBag.Classes = logic.Classes();
                ViewBag.Shicvas = logic.Shicvas();
                ViewBag.DefaultCityId = logic.DefaultCityId;

            }
            if (ViewBag.DefaultCityId > 0)
            {
                using (var logic5 = new tblStreetsLogic())
                {
                    ViewBag.DefaultCity = logic5.GetCityById(ViewBag.DefaultCityId);
                }
            }
            using (var logic2 = new LineLogic())
            {
                ViewBag.Lines = logic2.GetList();
            }
            using (var logic3 = new StationsLogic())
            {
                ViewBag.Stations = logic3.GetList();
            }
            using (var logic4 = new tblFamilyLogic())
            {
                ViewBag.Families = JsonConvert.SerializeObject(logic4.GetAll().Select(z => new
                {
                    Id = z.familyId,
                    Name = z.parent1FirstName + " " + z.parent1LastName +
                           (z.oneParentOnly ? "" : " / " + z.parent2FirstName + " " + z.parent2LastName)
                }));
            }
            using (var logic5 = new tblSchoolLogic())
            {
                ViewBag.Schools = JsonConvert.SerializeObject(logic5.GetList().Select(z => new
                {
                    Id = z.id,
                    Name = z.name
                }));
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public JsonResult StudentsForTable(GridSettings grid)
        {
            var rawClasses = Request["Classes"];
            var rawShicvas = Request["Shicvas"];
            var rawLines = Request["Lines"];
            var rawStations = Request["Stations"];
            var req = new StudentSearchRequest
            {
                SortColumn = grid.SortColumn,
                SortOrder = grid.SortOrder,
                PageNumber = grid.PageIndex,
                PageSize = grid.PageSize,
                StudentId = Request["Id"].Trim(),
                Name = Request["Name"].Trim(),
                Class = rawClasses.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                Shicva = rawShicvas.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                LineIds = rawLines.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                StationIds = rawStations.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                City = Request["City"].Trim().ToLower(),
                Street = Request["Street"].Trim().ToLower(),
                House = Request["House"].Trim().ToLower(),
                Active = int.Parse(Request["Active"]),
                registrationStatus = int.Parse(Request["registrationStatus"]),
                PayStatus = int.Parse(Request["PayStatus"]),
                Subcidy = int.Parse(Request["Subcidy"]),
                SibilingAtSchool = int.Parse(Request["Sibiling"]),
                SpecialRequest = int.Parse(Request["Request"]),
                DistanceFromSchoolFrom = int.Parse(Request["DFSFrom"]) * 1000,
                DistanceFromSchoolTo = int.Parse(Request["DFSTo"]) * 1000,
                DistanceFromStationFrom = int.Parse(Request["DFStFrom"]),
                DistanceFromStationTo = int.Parse(Request["DFStTo"]),
                Direction = int.Parse(Request["Direction"])
            };
            var total = 0;
            var logic = new tblStudentLogic();
            var lst = logic.GetStudentsForTable(req, out total);
            var jsonData = new
            {
                total = (int)Math.Ceiling((double)total / grid.PageSize),
                page = grid.PageIndex,
                records = total,
                rows = (from stud in lst
                        select
                            new
                            {
                                id = stud.Id,
                                cell = new string[] {  
                            stud.StudentId,
                            stud.FirstName,
                            stud.LastName,
                            stud.Class,
                            stud.Shicva,
                            stud.Address,
                            stud.registrationStatus.HasValue?stud.registrationStatus.ToString():"null",
                            stud.PayStatus.HasValue?stud.PayStatus.ToString():"null",
                            stud.Active.HasValue?stud.Active.ToString():"null",
                            stud.SibilingAtSchool.HasValue?stud.SibilingAtSchool.ToString():"null",
                            stud.SpecialRequest.ToString(),
                            stud.DistanceToSchool.ToString(),
                            stud.LineName
                        }
                            })

            };
            var r = new JsonResult { Data = jsonData };
            return r;
        }

        [HttpPost]
        [Authorize]
        public JsonResult StudentsForFTable(GridSettings grid)
        {


            var total = 0;
            var logic = new tblStudentLogic();
            var familyId = 0;
            int? studentId = null;
            if (Request["familyId"] != null)
            {
                familyId = Int32.Parse(Request["familyId"]);
            }
            else
            {
                studentId = Int32.Parse(Request["studentId"]);
                var st = logic.getStudentByPk(studentId.Value);
                familyId = st.familyId;
            }

            var lst = logic.GetStudentsForFamily(familyId, studentId, grid.PageIndex, grid.PageSize, grid.SortColumn,
                grid.SortOrder, out total);
            var jsonData = new
            {
                total = (int)Math.Ceiling((double)total / grid.PageSize),
                page = grid.PageIndex,
                records = total,
                rows = (from stud in lst
                        select
                            new
                            {
                                id = stud.Id,
                                cell = new string[] {  
                            stud.StudentId,
                            stud.Name,
                            stud.Class,
                            stud.Shicva,
                            stud.Address}
                            })

            };
            var r = new JsonResult { Data = jsonData };
            return r;
        }



        [HttpPost]
        [Authorize]
        public JsonResult GetLinesForStudent(GridSettings grid)
        {
            var studentId = Int32.Parse(Request["studentId"]);
            var total = 0;
            var logic = new tblStudentLogic();
            var lst = logic.GetLinesForStudent(studentId, grid.PageIndex, grid.PageSize, grid.SortColumn,
                grid.SortOrder, out total);
            var jsonData = new
            {
                total = (int)Math.Ceiling((double)total / grid.PageSize),
                page = grid.PageIndex,
                records = total,
                rows = (from stud in lst
                        select
                            new
                            {
                                id = stud.Id,
                                cell = new string[] {  
                            stud.Color,
                            stud.Number,
                            stud.Name,
                            stud.Direction==0?"TO":"FROM",
                            stud.Date.HasValue?stud.Date.Value.ToString("HH:mm dd-MM-yyyy"):"--"}
                            })

            };
            var r = new JsonResult { Data = jsonData };
            return r;
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveFamily(tblFamily data)
        {
            data.oneParentOnly = (Request.Form["oneParentOnly"] == "on");
            data.parent1GetAlertByEmail = (Request.Form["parent1GetAlertByEmail"] == "on");
            data.parent1GetAlertBycell = (Request.Form["parent1GetAlertBycell"] == "on");
            data.parent2GetAlertByEmail = (Request.Form["parent2GetAlertByEmail"] == "on");
            data.parent2GetAlertBycell = (Request.Form["parent2GetAlertBycell"] == "on");
            data.subsidy = (Request.Form["subsidy"] == "on");

            var id = 0;
            if (data.familyId == 0)
            {
                id = tblFamilyLogic.createFamily(data);
            }
            else
            {
                using (var logic = new tblFamilyLogic())
                {
                    var fm = logic.GetFamilyById(data.familyId);
                    fm.oneParentOnly = data.oneParentOnly;
                    fm.parent1Type = data.parent1Type;
                    fm.parent1FirstName = data.parent1FirstName;
                    fm.parent1LastName = data.parent1LastName;
                    fm.parent1Email = data.parent1Email;
                    fm.parent1GetAlertByEmail = data.parent1GetAlertByEmail;
                    fm.parent1CellPhone = data.parent1CellPhone;
                    fm.parent1GetAlertBycell = data.parent1GetAlertBycell;

                    fm.parent2Type = data.parent2Type;
                    fm.parent2FirstName = data.parent2FirstName;
                    fm.parent2LastName = data.parent2LastName;
                    fm.parent2Email = data.parent2Email;
                    fm.parent2GetAlertByEmail = data.parent2GetAlertByEmail;
                    fm.parent2CellPhone = data.parent2CellPhone;
                    fm.parent2GetAlertBycell = data.parent2GetAlertBycell;

                    fm.subsidy = data.subsidy;


                    tblFamilyLogic.update(fm);
                }
            }


            return null;
        }

        [HttpPost]
        [Authorize]
        public JsonResult SaveStudent(tblStudent data)
        {
            data.Active = (Request.Form["Active"] == "on");
            data.subsidy = (Request.Form["subsidy"] == "on");
            data.siblingAtSchool = (Request.Form["siblingAtSchool"] == "on");
            data.GetAlertByEmail = (Request.Form["GetAlertByEmail"] == "on");
            data.GetAlertByCell = (Request.Form["GetAlertByCell"] == "on");
            data.specialRequest = (Request.Form["specialRequest"] == "on");
            if (data.pk == 0)
            {
                tblStudentLogic.create(data);
            }
            else
            {
                using (var logic = new tblStudentLogic())
                {
                    var st = logic.getStudentByPk(data.pk);
                    if (st.city != data.city || st.street != data.street || st.houseNumber != data.houseNumber)
                    {
                        //address changed
                        st.Lat = null;
                        st.Lng = null;
                        st.distanceFromSchool = 0;
                    }

                    st.familyId = data.familyId;
                    st.siblingAtSchool = data.siblingAtSchool;
                    st.firstName = data.firstName;
                    st.lastName = data.lastName;
                    st.@class = data.@class;
                    st.Shicva = data.Shicva;
                    st.city = data.city;
                    st.street = data.street;
                    st.houseNumber = data.houseNumber;
                    st.zipCode = data.zipCode;
                    st.Email = data.Email;
                    st.GetAlertByEmail = data.GetAlertByEmail;
                    st.CellPhone = data.CellPhone;
                    st.GetAlertByCell = data.GetAlertByCell;
                    st.specialRequest = data.specialRequest;
                    st.request = data.request;
                    st.Active = data.Active;
                    st.subsidy = data.subsidy;
                    st.streetId = data.streetId;
                    st.cityId = data.cityId;

                    tblStudentLogic.update(st);
                }
            }

            return null;
        }

        /// <summary>
        /// Get Family for stuudent
        /// </summary>
        /// <param name="id">Student Id</param>
        /// <returns>Can be null</returns>
        [HttpGet]
        [Authorize]
        public string GetFamily(string id)
        {
            tblFamily res = null;
            using (var logic = new tblFamilyLogic())
            {
                res = logic.GetFamilyByStudentId(id.Trim());
            }
            return JsonConvert.SerializeObject(res);
        }

        [HttpGet]
        [Authorize]
        public string GetStudent(string id)
        {
            tblStudent res = null;
            using (var logic = new tblStudentLogic())
            {
                int iid;
                if (int.TryParse(id, out iid))
                {
                    res = logic.getStudentByPk(iid);
                }
            }
            return JsonConvert.SerializeObject(res);
        }

        [HttpGet]
        [Authorize]
        public void DeleteStudent(string id)
        {
            int iid;
            if (int.TryParse(id, out iid))
            {
                using (var logic = new tblStudentLogic())
                {
                    logic.Delete(iid);
                }
            }
        }


        [HttpGet]
        [Authorize]
        public ActionResult create()
        {

            int familyId = (int)Session["familyId"];//todo  take from session
            tblSystem v;
            if (HttpRuntime.Cache["currentYear"] == null)
            {
                v = tblSystemLogic.getSystemValueByKey("currentRegistrationYear");
                HttpRuntime.Cache["currentYear"] = v.strValue;
            }
            int yearRegistration = int.Parse(HttpRuntime.Cache["currentYear"].ToString());

            using (tblStudentLogic student = new tblStudentLogic())
            {

                tblStudent c = new tblStudent();
                c.CellConfirm = false;
                c.EmailConfirm = false;
                c.familyId = familyId;
                c.yearRegistration = yearRegistration;
                c.GetAlertByCell = false;
                c.GetAlertByEmail = false;
                c.siblingAtSchool = false;
                c.specialRequest = false;
                c.city = DictExpressionBuilderSystem.Translate("city.TelAviv");



                tblStudent n = student.getStudentByFamilyId(familyId);
                if (n != null)
                {


                    //c.cityCode = n.cityCode;
                    c.street = n.street;
                    c.lastName = n.lastName;
                    c.houseNumber = n.houseNumber;
                }


                tblStreet r = new Business_Logic.tblStreet();

                List<tblStreet> s = new List<tblStreet>();//TODO get tblStreet from cache

                studentViewModel vm = new studentViewModel()
                {
                    EditableTblStudents = c,
                    clas = student.clas()
                };
                return View(vm);
            }
        }
        internal static async Task<tblStudent> geoCoding(tblStudent c)
        {
            var address = "ישראל , " + c.city + " ," + c.street + " ," + c.houseNumber;
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

            Task<string> t = new Task<string>(() =>
            {
                var request = WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());
                var result = xdoc.Element("GeocodeResponse").Element("result");
                var locationElement = result.Element("geometry").Element("location");
                var lat = locationElement.Element("lat");
                var lng = locationElement.Element("lng");
                c.Lat = (double)lat;
                c.Lng = (double)lng;
                return "ok";
                // return c;
            });
            t.Start();
            string d = await t;
            return c;
        }

        [HttpPost]
        [Authorize]
        public JsonResult CitiesAutocomplete(string term)
        {
            List<SimpleItem> lst;
            using (var logic = new tblStudentLogic())
            {
                lst = logic.CitiesAutocomplete(term);
            }
            return new JsonResult() { Data = lst };
        }

        [HttpPost]
        [Authorize]
        public JsonResult StreetsAutocomplete(string term, int cityId)
        {
            List<SimpleItem> lst;
            using (var logic = new tblStudentLogic())
            {
                lst = logic.StreetsAutocomplete(term, cityId);
            }
            return new JsonResult() { Data = lst };
        }

        [HttpPost]
        [Authorize]
        public FileContentResult GetExcel()
        {

            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));

            var rawClasses = Request["Classes"];
            var rawShicvas = Request["Shicvas"];
            var rawLines = Request["Lines"];
            var rawStations = Request["Stations"];
            var req = new StudentSearchRequest
            {
                PageSize = Int32.MaxValue,
                PageNumber = 1,
                SortColumn = Request["SortColumn"],
                SortOrder = Request["SortOrder"],
                StudentId = Request["Id"].Trim(),
                Name = Request["Name"].Trim(),
                Class = rawClasses.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                Shicva = rawShicvas.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                LineIds = rawLines.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                StationIds = rawStations.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                City = Request["City"].Trim().ToLower(),
                Street = Request["Street"].Trim().ToLower(),
                House = Request["House"].Trim().ToLower(),
                Active = int.Parse(Request["Active"]),
                PayStatus = int.Parse(Request["PayStatus"]),
                Subcidy = int.Parse(Request["Subcidy"]),
                SibilingAtSchool = int.Parse(Request["Sibiling"]),
                SpecialRequest = int.Parse(Request["Request"]),
                DistanceFromSchoolFrom = int.Parse(Request["DFSFrom"]) * 1000,
                DistanceFromSchoolTo = int.Parse(Request["DFSTo"]) * 1000,
                DistanceFromStationFrom = int.Parse(Request["DFStFrom"]),
                DistanceFromStationTo = int.Parse(Request["DFStTo"]),
                Direction = int.Parse(Request["Direction"])
            };
            int ttl = 0;
            List<StudentFullInfo> lst = null;
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Students");
            worksheet.Cell("A1").Value = "Id";
            worksheet.Cell("B1").Value = "First Name";
            worksheet.Cell("C1").Value = "Last Name";
            worksheet.Cell("D1").Value = "Class";
            worksheet.Cell("E1").Value = "Shicva";
            worksheet.Cell("F1").Value = "Address";
            worksheet.Cell("G1").Value = "Payment";
            worksheet.Cell("H1").Value = "Active";
            worksheet.Cell("I1").Value = "Sibiling";
            worksheet.Cell("J1").Value = "Special request";
            worksheet.Cell("K1").Value = "Distance to School";
            worksheet.Cell("L1").Value = "Line & station";
            worksheet.Cell("M1").Value = "Email";
            worksheet.Cell("N1").Value = "Phone";
            worksheet.Cell("O1").Value = "Subsidy";
            worksheet.Cell("P1").Value = "Year Registration";
            worksheet.Cell("Q1").Value = "School";

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

                var row = 2;
                foreach (var stud in lst)
                {

                    worksheet.Cell(row, "A").Value = stud.StudentId;
                    worksheet.Cell(row, "B").Value = stud.FirstName;
                    worksheet.Cell(row, "C").Value = stud.LastName;
                    worksheet.Cell(row, "D").Value = stud.Class;
                    worksheet.Cell(row, "E").Value = stud.Shicva;
                    worksheet.Cell(row, "F").Value = stud.Address;
                    worksheet.Cell(row, "G").Value = stud.PayStatus == true ? "Yes" : "No";
                    worksheet.Cell(row, "H").Value = stud.Active == true ? "Yes" : "No";
                    worksheet.Cell(row, "I").Value = stud.SibilingAtSchool == true ? "Yes" : "No";
                worksheet.Cell(row, "J").Value = stud.SpecialRequest == true ? "Yes" : "No";
                    worksheet.Cell(row, "K").Value = stud.DistanceToSchool + " m.";
                    worksheet.Cell(row, "L").Value = stud.LineName;

                    row++;
                }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            var sr = new BinaryReader(ms);
            return File(sr.ReadBytes((int)ms.Length), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx"); ;

        }

        [HttpGet]
        [Authorize]
        public ActionResult Print()
        {
            var rawClasses = Request["Classes"];
            var rawShicvas = Request["Shicvas"];
            var rawLines = Request["Lines"];
            var rawStations = Request["Stations"];
            var req = new StudentSearchRequest
            {
                PageSize = Int32.MaxValue,
                PageNumber = 1,
                SortColumn = Request["SortColumn"],
                SortOrder = Request["SortOrder"],
                StudentId = Request["Id"].Trim(),
                Name = Request["Name"].Trim(),
                Class = rawClasses.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                Shicva = rawShicvas.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries),
                LineIds = rawLines.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                StationIds = rawStations.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray(),
                City = Request["City"].Trim().ToLower(),
                Street = Request["Street"].Trim().ToLower(),
                House = Request["House"].Trim().ToLower(),
                Active = int.Parse(Request["Active"]),
                PayStatus = int.Parse(Request["PayStatus"]),
                Subcidy = int.Parse(Request["Subcidy"]),
                SibilingAtSchool = int.Parse(Request["Sibiling"]),
                SpecialRequest = int.Parse(Request["Request"]),
                DistanceFromSchoolFrom = int.Parse(Request["DFSFrom"]) * 1000,
                DistanceFromSchoolTo = int.Parse(Request["DFSTo"]) * 1000,
                DistanceFromStationFrom = int.Parse(Request["DFStFrom"]),
                DistanceFromStationTo = int.Parse(Request["DFStTo"]),
                Direction = int.Parse(Request["Direction"])
            };

            using (var logic = new tblStudentLogic())
            {
                var ttl = 0;
                ViewBag.List = logic.GetStudentsForTable(req, out ttl);
            }
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult create(studentViewModel c)
        {
            if (HttpRuntime.Cache["currentYear"] == null)
            {
                using (tblSystemLogic system = new tblSystemLogic())
                {
                    string x = tblSystemLogic.getSystemValueByKey("currentRegistrationYear").ToString();
                    HttpRuntime.Cache.Insert("currentYear", x, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(50), CacheItemPriority.High, null);
                }
            }
            int yearRegistration = int.Parse(HttpRuntime.Cache["currentYear"].ToString());
            if (tblStudentLogic.checkIfIdExist(c.EditableTblStudents.studentId, yearRegistration))
            {
                ;
                ViewBag.message = DictExpressionBuilderSystem.Translate("message.studentIsAllredyRegister");
                //ViewBag.message = "התלמיד כבר רשום במערכת לשנה זו - לא ניתן לבצע רישום ";
                return View(c);
            }

            if (c != null)
            {
                if (!checkstreet(c.EditableTblStudents.street))
                {
                    //ViewBag.message = "יש לבחור שם רחוב מהרשימה";
                    ViewBag.message = DictExpressionBuilderSystem.Translate("message.selectStreetFromList");
                    return View(c);
                }

                tblStudent v = new tblStudent();
                v = c.EditableTblStudents;
                Task<tblStudent> x = geoCoding(v);
                //v.Lng = x.Result.Lng;//do not nedded since lng lat will get at the admin atachment to station
                //v.Lat = x.Result.Lat;
                int familyId = (int)Session["familyId"];


                v.familyId = familyId;
                v.yearRegistration = yearRegistration;
                v.registrationStatus = false;
                v.Active = true;
                v.Color = "";
                tblStudentLogic.create(v);

                return RedirectToAction("index", "Family");


            }
            else
                return Redirect("~/account/unAutorise");//real check is by Js in client side- if we here there is asecurity problem!


        }

        [HttpGet]
        public ActionResult update(int id)//id= studentPk
        {
            int studentPk = id;
            int familyId = (int)Session["familyId"];
            // int familyId = 11;//todo  take from session
            // int yearRegistration = 2017;///todo  take from cache
            //check if this studentPk has familyId the same as in the sessio-else return null
            using (tblStudentLogic student = new tblStudentLogic())
            {

                tblStudent c = student.getStudentByPk(studentPk);
                if (c == null || c.familyId != familyId)//for security
                    return null;
                //   tblStreet r = new Business_Logic.tblStreet();

                List<tblStreet> s = new List<tblStreet>();//TODO get tblStreet from cache

                studentViewModel vm = new studentViewModel()
                {
                    EditableTblStudents = c,
                    CellConfirm = c.CellConfirm.HasValue ? c.CellConfirm.Value : false,
                    GetAlertByCell = c.GetAlertByCell.HasValue ? c.GetAlertByCell.Value : false,
                    EmailConfirm = c.EmailConfirm.HasValue ? c.EmailConfirm.Value : false,
                    GetAlertByEmail = c.GetAlertByEmail.HasValue ? c.GetAlertByEmail.Value : false,
                    paymentStatus = c.paymentStatus.HasValue ? c.paymentStatus.Value : false,
                    siblingAtSchool = c.siblingAtSchool.HasValue ? c.siblingAtSchool.Value : false,
                    specialRequest = c.specialRequest.HasValue ? c.specialRequest.Value : false,
                    clas = student.clas()
                };
                return View(vm);
            }

            //get family name from tblFamily
            //get list of street to viewbag from cashe
            //check if the student was register last year get the relevant data- it may be the same student
            //else shceck if student has brothers - take the relevant data

            //show  view  with the relevant data 



        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult update(studentViewModel c)
        {


            if (c != null)
            {

                using (tblStudentLogic student = new tblStudentLogic())
                {
                    int familyId = (int)Session["familyId"];
                    tblStudent v = student.getStudentByPk(c.EditableTblStudents.pk);
                    if (v.familyId == familyId && v.pk == c.EditableTblStudents.pk)//familiy from session + check if its the same student
                    {
                        if (v.street != c.EditableTblStudents.street || v.houseNumber != c.EditableTblStudents.houseNumber)
                            v.registrationStatus = false;//Must Have administration confirm for changing the buss station for student
                        v.street = c.EditableTblStudents.street;
                        if (!checkstreet(c.EditableTblStudents.street))
                        {
                            ViewBag.message = DictExpressionBuilderSystem.Translate("message.selectStreetFromList");
                            return View(c);
                        }
                        v.houseNumber = c.EditableTblStudents.houseNumber;
                        v.Shicva = c.EditableTblStudents.Shicva;
                        v.@class = c.EditableTblStudents.@class;
                        v.subsidy = c.EditableTblStudents.subsidy;
                        if (v.Email != c.EditableTblStudents.Email)
                        {
                            v.Email = c.EditableTblStudents.Email;
                            v.GetAlertByEmail = c.EditableTblStudents.GetAlertByEmail;
                            v.EmailConfirm = false;
                        }
                        if (v.CellPhone != c.EditableTblStudents.CellPhone)
                        {
                            v.CellPhone = c.EditableTblStudents.CellPhone;
                            v.GetAlertByCell = c.EditableTblStudents.GetAlertByCell;
                            v.CellConfirm = false;
                        }
                        //Task<tblStudent> x = geoCoding(v);
                        //v.Lng = x.Result.Lng;
                        //v.Lat = x.Result.Lat;
                        v.lastUpdate = DateTime.Today;
                        tblStudentLogic.update(v);
                    }
                }
                return RedirectToAction("index", "Family");
            }
            else
                return Redirect("~/account/unAutorise");//real check is by Js in client side- if we here there is asecurity problem!
        }
        public bool checkstreet(string streetName)
        {
            using (tblStreetsLogic street = new tblStreetsLogic())
            {
                return street.IsExist(streetName);
            }

        }


    }
}





