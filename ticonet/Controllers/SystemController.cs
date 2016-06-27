using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Business_Logic;
using log4net;

namespace ticonet
{
    public class SystemController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SystemController));

        //public class ApplicationUser : IdentityUser
        //{
        //    public string mail { get; set; }
        //    public bool ConfirmedEmail { get; set; }
        //}
        [HttpPost]
        [Authorize]
        public JsonResult ConfirmEmail(string emailHolder)
        {
            string id = Convert.ToString(Session["familyId"]);
            string email = "";
            if (emailHolder == "parent1")
            {
                using (tblFamilyLogic family = new tblFamilyLogic())
                {
                    ViewBag.Years = tblYearsLogic.GetYears();
                    tblFamily c = family.GetFamilyById(int.Parse(id));
                    email = c.parent1Email;
                }
            }
            Task<string> x = sendConfirmationEmail(email, emailHolder, id);
            //if (x.Result == "ok")
            return Json(new { success = true });
            //else
            //    return Json(JsonRequestBehavior.DenyGet);
        }

        public async Task<string> sendConfirmationEmail(string email, string emailHolder, string familyId)
        {

            FormsAuthenticationTicket formsTicket = new FormsAuthenticationTicket(
                1,
                familyId,
                DateTime.Now,
                DateTime.Now.AddMinutes(60 * 5),
                true,
                emailHolder
                );
            string encryptedTicket = FormsAuthentication.Encrypt(formsTicket);

            MailMessage m = new MailMessage(
                new MailAddress("harshamaHasaot@gmail.com", "Web Registration"),
                new MailAddress(email));
            m.Subject = "Email confirmation";
            m.Body = string.Format("Dear {0}<BR/>" + DictExpressionBuilderSystem.Translate("message.pleaseclickontheLinkbelow") + ": <a href=\"{1}\" title=\"" + DictExpressionBuilderSystem.Translate("message.UserEmailConfirm") + "\">Click here</a>", email, Url.Action("ConfirmEmail", "System", new { Token = encryptedTicket, Email = email }, Request.Url.Scheme));
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Credentials = new NetworkCredential("harshamaHasaot@gmail.com", "zaqzaq8*");
            smtp.EnableSsl = true;
            //  await smtp.SendMailAsync(m);
            Task<string> t = new Task<string>(() =>
            {

                smtp.SendMailAsync(m);
                return "ok";
            });

            t.Start();
            string result = await t;
            return "ok";
        }


        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string Token, string Email)
        {
            if (Token != null)
            {

                Task<string> t = new Task<string>(() =>
                {
                    using (LoginLogic login = new LoginLogic())
                    {
                        FormsAuthenticationTicket formsTicket = FormsAuthentication.Decrypt(Token);
                        int familyId = int.Parse(formsTicket.Name);
                        string emailHolder = formsTicket.UserData;
                        //   string schoolName = tblSystemLogic.getSystemValueByKey("schoolName").value;

                        if (!formsTicket.Expired && tblFamilyLogic.checkIfFamilyExist(familyId))//take from db
                        {
                            using (tblFamilyLogic family = new tblFamilyLogic())
                            {
                                tblFamily c = family.GetFamilyById(familyId);
                                if (emailHolder == "parent1" && c.parent1Email == Email)
                                    c.parent1EmailConfirm = true;
                                if (emailHolder == "parent2" && c.parent2Email == Email)
                                    c.parent2EmailConfirm = true;
                                tblFamilyLogic.update(c);
                            }
                            //  return RedirectToAction("OK");
                            return "OK";
                        }
                        else
                            return "Error";
                        //return RedirectToAction("Error");
                    }
                });

                t.Start();
                string result = await t;
                if (result == "OK")
                    return RedirectToAction("OK");
                else
                    return RedirectToAction("Error");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        public async Task<bool> SignInAsync(string Email)//just example
        {
            await Task.Delay(1);
            return LoginLogic.confirmEmail(Email);


        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult OK()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        //[ValidateAntiForgeryToken]
        public JsonResult ConfirmCellPhonNumber(string cellNumber, string phoneHolder)
        {
            try
            {
                int cellCounter = 0;
                if (Session["cellCounter"] != null)
                    cellCounter = int.Parse(Session["cellCounter"].ToString());
                if (cellCounter > 3)
                    return Json(JsonRequestBehavior.DenyGet);
                cellCounter += 1;
                Session["cellCounter"] = cellCounter;
                Session["phoneHolder"] = phoneHolder;
                Random rnd = new Random();
                int kod = rnd.Next(1111, 9999);
                Session["cellNumber"] = cellNumber;
                Session["kod"] = kod;
                SmsController x = new SmsController();

                bool send = x.SendSms(cellNumber, kod.ToString());
                return Json(0);
            }
            catch
            {
                return Json(JsonRequestBehavior.DenyGet);
            }

        }
        [HttpPost]
        [Authorize]
        public JsonResult ConfirmCellPhonNumberupdate(string kod, string cellNumber)
        {
            try
            {
                string SessioncellNumber = null;
                string Sessionkod = null;
                if (Session["cellNumber"] != null && Session["kod"] != null)
                {
                    SessioncellNumber = Session["cellNumber"].ToString();
                    Sessionkod = Session["kod"].ToString();
                    if (kod == Sessionkod && cellNumber == SessioncellNumber)
                    {
                        int familyId = int.Parse(Session["familyId"].ToString());
                        string phoneHolder = Convert.ToString(Session["phoneHolder"]);


                        using (tblFamilyLogic family = new tblFamilyLogic())
                        {
                            tblFamily c = family.GetFamilyById(familyId);
                            if (phoneHolder == "parent1cell")
                                c.parent1CellConfirm = true;
                            if (phoneHolder == "parent2cell")
                                c.parent1CellConfirm = true;
                            tblFamilyLogic.update(c);
                        }

                        return Json(0);
                    }
                    return Json(JsonRequestBehavior.DenyGet);
                }
                return Json(JsonRequestBehavior.DenyGet);
            }
            catch
            {
                return Json(JsonRequestBehavior.DenyGet);
            }

        }
    }
}