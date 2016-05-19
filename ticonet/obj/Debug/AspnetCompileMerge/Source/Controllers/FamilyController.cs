using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Business_Logic;

namespace ticonet
{
    public class FamilyController : Controller
    {
        [Authorize]
        public ActionResult Index(int? year)//id=familyId
        {
            if (Session["familyId"] == null)//user did not create family form yet
            {
                return RedirectToAction("create");
            }
            int id = (int)Session["familyId"];
            using (tblFamilyLogic family = new tblFamilyLogic())
            {
                using (tblStudentLogic students = new tblStudentLogic())
                {
                    ViewBag.Years = tblYearsLogic.GetYears();
                    tblFamily c = family.GetFamilyById(id);


                    if (c != null)
                    {
                        //if emailconfirm = nul
                        if (c.parent1EmailConfirm == false)
                        {
                            if (c.parent1Email == (string)Session["mailRegistration"])
                            {
                                using (LoginLogic login = new LoginLogic())
                                {
                                    if (login.checkIfregisterd((string)Session["mailRegistration"]))
                                    {
                                        c.parent1EmailConfirm = true;
                                        tblFamilyLogic.update(c);
                                    }
                                }


                            }
                        }




                        List<tblStudent> s;
                        if (!year.HasValue)
                            s = students.GetStudentByFamilyIdAndYear(id);
                        else
                            s = students.GetStudentByFamilyIdAndYear(id, year.Value);

                        familyViewModel vm = new familyViewModel()
                        {
                            EditableTblFamily = c,
                            students = s
                        };

                        return View(vm);
                    }
                    return null;

                }
            }
        }

        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult create()
        {

            string email = (string)Session["mailRegistration"];



            if (!string.IsNullOrEmpty(email) && !tblFamilyLogic.checkEmailExist(email) && Regex.IsMatch(email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                using (tblFamilyLogic family = new tblFamilyLogic())
                {
                    tblFamily c = new tblFamily();
                    c.parent1Email = email;
                    c.iAgree = false;
                    c.parent1GetAlertBycell = false;
                    c.parent1GetAlertByEmail = false;

                    using (LoginLogic login = new LoginLogic())
                    {
                        if (login.checkIfregisterd(email))
                            c.parent1EmailConfirm = true;
                        else
                            c.parent1EmailConfirm = false;
                    }
                    c.parent1CellConfirm = false;
                    c.parent2GetAlertBycell = false;
                    c.parent2GetAlertByEmail = false;
                    c.parent2EmailConfirm = false;
                    c.parent2CellConfirm = false;
                    c.paymentOk = false;
                    c.date = DateTime.Today;
                    c.allredyUsed = false;
                    return View(c);

                }
            }
            else
                return Redirect("~/account/unAutorise");//real check is by Js in client side- if we here there is asecurity problem!
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult create(tblFamily c)
        {
            //check if id of form not allredy in the database

            if (tblFamilyLogic.checkIfIdExist(c.ParentId))
            {
                ViewBag.message = "מספר הזהות של מבצע ההרשמה כבר קיימת במערכת- לא ניתן להבצע רישום ";
                return View(c);
            }
            c.parent1Email = (string)Session["mailRegistration"];

            if (tblFamilyLogic.checkIfEmailExist(c.parent1Email))
            {
                ViewBag.message = "האימייל של מבצע ההרשמה כבר קיימת במערכת- לא ניתן להבצע רישום ";
                return View(c);
            }
            if (c.iAgree == false)
            {
                ViewBag.message = "יש לקרא ולאשר את התקנון  ";
                return View(c);
            }

            if (c != null)
            {


                int familyId = tblFamilyLogic.createFamily(c);
                string userName = Session["mailRegistration"].ToString();
                LoginLogic.updateFamilyId(userName, familyId);
                Session["familyId"] = familyId;
                return RedirectToAction("index", new { id = familyId });


            }
            else
                return Redirect("~/account/unAutorise");//real check is by Js in client side- if we here there is asecurity problem!
        }
        [HttpGet]
            public ActionResult update()//id= familyId
        {
            int familyId = (int)Session["familyId"];

            using (tblFamilyLogic family = new tblFamilyLogic())
            {
                tblFamily c = family.GetFamilyById(familyId);
                if (c.allredyUsed == null)
                    c.allredyUsed = false;
                familyViewModel vm = new familyViewModel()
                {
                    EditableTblFamily = c,
                    parent1CellConfirm = c.parent1CellConfirm,
                    parent1EmailConfirm = c.parent1EmailConfirm,
                    parent1CellGetAlert = c.parent1GetAlertBycell,
                    parent1EmailGetAlert = c.parent1GetAlertByEmail,
                    parentAgree = c.iAgree,
                    allredyUse = c.allredyUsed.Value

                };
                return View(vm);
            }
        }


        [HttpPost]
        public ActionResult update(familyViewModel c)
        {
            int familyId = (int)Session["familyId"];
            c.EditableTblFamily.familyId = familyId;
            if (c != null)
            {

                using (tblFamilyLogic family = new tblFamilyLogic())
                {

                    tblFamily v = family.GetFamilyById(familyId);

                    if (c.EditableTblFamily.parent1CellPhone != v.parent1CellPhone)
                    {
                        v.parent1CellConfirm = false;
                        v.parent1CellPhone = c.EditableTblFamily.parent1CellPhone;
                    }
                    if (c.EditableTblFamily.parent1Email != v.parent1Email)
                    {
                        v.parent1EmailConfirm = false;
                        v.parent1Email = c.EditableTblFamily.parent1Email;
                    }
                    if (c.EditableTblFamily.parent2Type != null)
                        v.parent2Type = c.EditableTblFamily.parent2Type;

                    if (c.EditableTblFamily.parent2FirstName != null)
                        v.parent2FirstName = c.EditableTblFamily.parent2FirstName;

                    if (c.EditableTblFamily.parent2LastName != null)
                        v.parent2LastName = c.EditableTblFamily.parent2LastName;

                    if (c.EditableTblFamily.parent2Email != null)
                        v.parent2Email = c.EditableTblFamily.parent2Email;

                    if (c.EditableTblFamily.parent2CellPhone != null)
                        v.parent2CellPhone = c.EditableTblFamily.parent2CellPhone;
                    v.allredyUsed = c.allredyUse;
                    v.LastUpdate = DateTime.Today;
                    tblFamilyLogic.update(v);
                }
                return RedirectToAction("index", "Family");
            }
            else
                return Redirect("~/account/unAutorise");//real check is by Js in client side- if we here there is asecurity problem!
        }
    }
}
