
using System.Web;

using System.Web.Mvc;
using log4net;
using System.Web.Security;
using Business_Logic;
using Microsoft.AspNet.Identity.EntityFramework;




namespace ticonet
{
    public class AccountController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(AccountController));
        public class ApplicationUser : IdentityUser
        {
            public string mail { get; set; }
            public bool ConfirmedEmail { get; set; }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid || model.userName!="admin" )
                return View();
            using (LoginLogic login = new LoginLogic())
            {
                if (login.IsExist(model.userName, FormsAuthentication.HashPasswordForStoringInConfigFile(model.password, "sha1")))
                {
                    //if mailconfirm ok
                  
                        FormsAuthentication.RedirectFromLoginPage(model.userName, model.RememberMe);
                        Session["mailRegistration"] = model.userName;
                       // Login c =login.getLogin(model.userName, FormsAuthentication.HashPasswordForStoringInConfigFile(model.password, "sha1"));
                     
                    //   int? familyId = LoginLogic.getFamilyId(model.userName, FormsAuthentication.HashPasswordForStoringInConfigFile(model.password, "sha1")).familyId;
                       //if (familyId.HasValue)
                       // { //redirect to action family}
                       //     Session["familyId"] = familyId;
                       //     return RedirectToAction("index", "Family");
                       // }
                       // else
                       //     return RedirectToAction("create", "Family");
               

                }
                else
                    ViewBag.message = DictExpressionBuilderSystem.Translate("message.IncorectuserNameorPassword");
                //ViewBag.message = "Incorect userName or Password";
            }
            return View();
        }

        public ActionResult Signout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult unAutorise()
        {
            // HttpRuntime.Cache.Remove("Menu" + SessionHelper.UserTypeName);


            //  SessionHelper.ClearData();
            Session.Clear();
            Session.Abandon();
            //-------------
            HttpContext.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.AddHeader("Pragma", "no-cache");
            HttpContext.Response.AddHeader("Expires", "0");
            FormsAuthentication.SignOut();
            //------
            Session.Clear();
            return View();
        }


        [AllowAnonymous]
        public ActionResult Confirm(string Email)
        {
            System.Threading.Thread.Sleep(3000);
            ViewBag.Title = DictExpressionBuilderSystem.Translate("message.ConfirmEmailAddressSent");
            ViewBag.message = DictExpressionBuilderSystem.Translate("message.PleasecheckyourEmailInbox");
            ViewBag.Email = Email;
            return View();
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View();

        //    string SchoolCode;

        //    if (HttpRuntime.Cache["SchoolCode"] == null)
        //    {
        //        using (tblSystemLogic system = new tblSystemLogic())
        //        {
        //            SchoolCode = tblSystemLogic.getSystemValueByKey("SchoolCode").value.ToString();
        //            HttpRuntime.Cache.Insert("SchoolCode", SchoolCode, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(500), CacheItemPriority.High, null);
        //        }
        //    }

        //    SchoolCode = HttpRuntime.Cache["SchoolCode"].ToString();

        //    if (SchoolCode != model.entranceCode)
        //    {
        //        ViewBag.message = DictExpressionBuilderSystem.Translate("message.YoumustentertherightSchoolKod");
        //        //ViewBag.message = "You must enter the right School Kod";
        //        return View();
        //    }


        //    using (LoginLogic login = new LoginLogic())
        //    {
        //        if (!login.IsExist(model.userName))
        //        {
        //            login.Register(model.userName, FormsAuthentication.HashPasswordForStoringInConfigFile(model.password, "sha1"), model.password);//not secure jest for testing period
        //        }
        //        else
        //        {
        //            ViewBag.message = DictExpressionBuilderSystem.Translate("message.Thisusernameisalreadytaken");
        //            return View();
        //        }
        //    }
        //    if (!Regex.IsMatch(model.userName, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
        //    {
        //        ViewBag.message = DictExpressionBuilderSystem.Translate("message.TheusernamemustbeyourE-mail");
        //        return View();
        //    }
        //    FormsAuthenticationTicket formsTicket = new FormsAuthenticationTicket(
        //        1,
        //        model.userName,
        //        DateTime.Now,
        //        DateTime.Now.AddMinutes(60 * 5),
        //        true,
        //        "tichonet"//for dif with many school in the future

        //        );
        //    string encryptedTicket = FormsAuthentication.Encrypt(formsTicket);
        //    var user = new ApplicationUser() { UserName = model.userName };
        //    user.Email = model.userName;
        //    user.ConfirmedEmail = true;
        //    string EmailAdress = tblSystemLogic.getSystemValueByKey("EmailAdress").value;
        //    string Password = tblSystemLogic.getSystemValueByKey("Password").value;
        //    string mailServer = tblSystemLogic.getSystemValueByKey("mailServer").value;
        //    System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
        //        new System.Net.Mail.MailAddress("harshamaHasaot@gmail.com", "Web Registration"),
        //        new System.Net.Mail.MailAddress(user.Email));
        //    m.Subject = DictExpressionBuilderSystem.Translate("message.Emailconfirmation");
        //    m.Body = string.Format("Dear {0}<BR/>" + DictExpressionBuilderSystem.Translate("message.pleaseclickonthebelow") + ": <a href=\"{1}\" title=\"" + DictExpressionBuilderSystem.Translate("message.UserEmailConfirm") + "\">{1}</a>", user.UserName, Url.Action("ConfirmEmail", "Account", new { Token = encryptedTicket, Email = user.Email }, Request.Url.Scheme));
        //    m.IsBodyHtml = true;
        //    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(mailServer);
        //    smtp.Credentials = new System.Net.NetworkCredential(EmailAdress, Password);
        //    smtp.EnableSsl = true;
        //    await smtp.SendMailAsync(m);
        //    return RedirectToAction("Confirm", "Account", new { Email = user.Email });
        //}


    
        public ActionResult Error()
        {
            return View();
        }

    }
}

