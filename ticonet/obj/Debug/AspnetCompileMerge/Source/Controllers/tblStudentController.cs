using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Business_Logic;


namespace ticonet
{
    public class tblStudentController : Controller
    {
        // GET: tblStudent
        public ActionResult Index()
        {
            return View();
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
                HttpRuntime.Cache["currentYear"] = v.value;
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

            //get family name from tblFamily
            //get list of street to viewbag from cashe
            //check if the student was register last year get the relevant data- it may be the same student
            //else shceck if student has brothers - take the relevant data

            //show  view  with the relevant data 



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
                    ViewBag.message =  DictExpressionBuilderSystem.Translate("message.selectStreetFromList");
                    return View(c);
                }

                tblStudent v = new tblStudent();
                v = c.EditableTblStudents;
                int familyId = (int)Session["familyId"];


                v.familyId = familyId;
                v.yearRegistration = yearRegistration;
                v.registrationStatus = false;
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
                if (c==null || c.familyId != familyId)//for security
                    return null;
             //   tblStreet r = new Business_Logic.tblStreet();

                List<tblStreet> s = new List<tblStreet>();//TODO get tblStreet from cache

                studentViewModel vm = new studentViewModel()
                {
                    EditableTblStudents = c,
                    CellConfirm =c.CellConfirm.HasValue? c.CellConfirm.Value:false,
                    GetAlertByCell = c.GetAlertByCell.HasValue?c.GetAlertByCell.Value:false,
                    EmailConfirm = c.EmailConfirm.HasValue ? c.EmailConfirm.Value : false,
                    GetAlertByEmail = c.GetAlertByEmail.HasValue ? c.GetAlertByEmail.Value : false,
                    paymentStatus = c.paymentStatus.HasValue ? c.paymentStatus.Value : false,
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





