using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using System.Web;

using System.Web.Caching;
using System.Web.Mvc;

using System.Web.Security;
using Business_Logic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet;
using System.Data.Entity;




namespace ticonet
{
    public class AccountController : Controller
    {

        public class ApplicationUser : IdentityUser
        {
            public string mail { get; set; }
            public bool ConfirmedEmail { get; set; }
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View();
            using (LoginLogic login = new LoginLogic())
            {
                if (login.IsExist(model.userName,FormsAuthentication.HashPasswordForStoringInConfigFile( model.password,"sha1")))
                {
                    //if mailconfirm ok
                    if (login.checkIfregisterd(model.userName))
                    {
                        FormsAuthentication.RedirectFromLoginPage(model.userName, model.RememberMe);
                        Session["mailRegistration"] = model.userName;
                        int? familyId = LoginLogic.getFamilyId(model.userName, FormsAuthentication.HashPasswordForStoringInConfigFile(model.password, "sha1")).familyId;
                        if (familyId.HasValue)
                        { //redirect to action family}
                            Session["familyId"] = familyId;
                          return  RedirectToAction("index", "Family");
                        }
                        else
                            return RedirectToAction("create", "Family");
                    }
                    else
                    {
                        LoginLogic.deleteByEmail(model.userName);
                        ViewBag.message = DictExpressionBuilderSystem.Translate("message.Youdidntconfirmyouremail");
                        //ViewBag.message = "You didnt confirm your email - try to register again and to confirm your email";
                    }

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
        public ActionResult Register()
        {
            return View();
        }
        //public async Task<ActionResult> Register(RegisterModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser() { UserName = model.userName };
        //        user.Email = model.userName;
        //        user.ConfirmedEmail = false;
        //        var result = await UserManager.CreateAsync(user, model.password);
        //        if (result.Succeeded)
        //        {
        //            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
        //                new System.Net.Mail.MailAddress("harshamaHasaot@gmail.com", "Web Registration"),
        //                new System.Net.Mail.MailAddress(user.Email));
        //            m.Subject = "Email confirmation";
        //            m.Body = string.Format("Dear {0}<BR/>Thank you for your registration, please click on the below link to complete your registration: <a href=\"{1}\" title=\"User Email Confirm\">{1}</a>", user.UserName, Url.Action("ConfirmEmail", "Account", new { Token = user.Id, Email = user.Email }, Request.Url.Scheme));
        //            m.IsBodyHtml = true;
        //            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com");
        //            smtp.Credentials = new System.Net.NetworkCredential("harshamaHasaot@gmail.com", "zaqzaq8*");
        //            smtp.EnableSsl = true;
        //            smtp.Send(m);
        //            return RedirectToAction("Confirm", "Account", new { Email = user.Email });
        //        }
        //        else
        //        {
        //            AddErrors(result);
        //        }
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        [AllowAnonymous]
        public ActionResult Confirm(string Email)
        {
            System.Threading.Thread.Sleep(3000);
            ViewBag.Title = DictExpressionBuilderSystem.Translate("message.ConfirmEmailAddressSent");
            ViewBag.message = DictExpressionBuilderSystem.Translate("message.PleasecheckyourEmailInbox");
            ViewBag.Email = Email;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        //public ActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
                return View();

            string SchoolCode;

            if (HttpRuntime.Cache["SchoolCode"] == null)
            {
                using (tblSystemLogic system = new tblSystemLogic())
                {
                    SchoolCode = tblSystemLogic.getSystemValueByKey("SchoolCode").value.ToString();
                    HttpRuntime.Cache.Insert("SchoolCode", SchoolCode, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(500), CacheItemPriority.High, null);
                }
            }

            SchoolCode = HttpRuntime.Cache["SchoolCode"].ToString();

            if (SchoolCode != model.entranceCode)
            {
                ViewBag.message = DictExpressionBuilderSystem.Translate("message.YoumustentertherightSchoolKod");
                //ViewBag.message = "You must enter the right School Kod";
                return View();
            }


            if (!Regex.IsMatch(model.userName, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"))
            {
                ViewBag.message = DictExpressionBuilderSystem.Translate("message.TheusernamemustbeyourE-mail");
       
                //ViewBag.message = "The username must be your E-mail";
                return View();
            }


            using (LoginLogic login = new LoginLogic())
            {
                if (!login.IsExist(model.userName))
                {

                    // login.Register(model.userName, FormsAuthentication.HashPasswordForStoringInConfigFile(model.password, "sha1")); - original
                    login.Register(model.userName, FormsAuthentication.HashPasswordForStoringInConfigFile(model.password, "sha1"), model.password);//not secure jest for testing period
                 //   Session["mailRegistration"] = model.userName;
                 //   FormsAuthentication.RedirectFromLoginPage(model.userName, false);//if we use it here the user is register without of mail autenticated
                }
                else
                {
                    ViewBag.message = DictExpressionBuilderSystem.Translate("message.Thisusernameisalreadytaken");
       
                    //ViewBag.message = "This username is already taken";
                    return View();
                }
            }

            //---string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());


            //byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            //byte[] key = Guid.NewGuid().ToByteArray();
            //string token = Convert.ToBase64String(time.Concat(key).ToArray());


            //string[] userData = new string[4];

            // fill the userData array with the information we need for subsequent requests
            //userData[0] = ...; // data we need
            //userData[1] = ...; // other data, etc

            // create a Forms Auth ticket with the username and the user data. 
            FormsAuthenticationTicket formsTicket = new FormsAuthenticationTicket(
                1,
                model.userName,
                DateTime.Now,
                DateTime.Now.AddMinutes(60*5),
                true,
                "tichonet"//for dif with many school in the future

                );
            string encryptedTicket = FormsAuthentication.Encrypt(formsTicket);

            //--

            //----------
            var user = new ApplicationUser() { UserName = model.userName };
            user.Email = model.userName;
            user.ConfirmedEmail = true;
            //var result = await UserManager.CreateAsync(user, model.password);
            //if (result.Succeeded)
            //{
            string EmailAdress = tblSystemLogic.getSystemValueByKey("EmailAdress").value;
            string Password = tblSystemLogic.getSystemValueByKey("Password").value;
            string mailServer = tblSystemLogic.getSystemValueByKey("mailServer").value;
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                new System.Net.Mail.MailAddress("harshamaHasaot@gmail.com", "Web Registration"),
                new System.Net.Mail.MailAddress(user.Email));
            //m.Subject = "Email confirmation";
            m.Subject = DictExpressionBuilderSystem.Translate("message.Emailconfirmation");
            //m.Body = string.Format("Dear {0}<BR/>Thank you for your registration, please click on the below link to complete your registration: <a href=\"{1}\" title=\"User Email Confirm\">{1}</a>", user.UserName, Url.Action("ConfirmEmail", "Account", new { Token = encryptedTicket, Email = user.Email }, Request.Url.Scheme));
            m.Body = string.Format("Dear {0}<BR/>" + DictExpressionBuilderSystem.Translate("message.pleaseclickonthebelow") + ": <a href=\"{1}\" title=\"" + DictExpressionBuilderSystem.Translate("message.UserEmailConfirm") + "\">{1}</a>", user.UserName, Url.Action("ConfirmEmail", "Account", new { Token = encryptedTicket, Email = user.Email }, Request.Url.Scheme));
     
          //  m.Body = string.Format("Dear {0}<BR/>{3} <a href=\"{1}\" title=\"{4}\">{1}</a>", user.UserName, Url.Action("ConfirmEmail", "Account", new { Token = encryptedTicket, Email = user.Email }, Request.Url.Scheme), DictExpressionBuilderSystem.Translate("message.pleaseclickonthebelow"), DictExpressionBuilderSystem.Translate("message.UserEmailConfirm"));
            m.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(mailServer);
            smtp.Credentials = new System.Net.NetworkCredential(EmailAdress, Password);
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
            // smtp.Send(m);
            return RedirectToAction("Confirm", "Account", new { Email = user.Email });
            //return View();
            //}
        }


        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string Token, string Email)
        {

            if (Token != null)
            {
                using (LoginLogic login = new LoginLogic())
                {
                    FormsAuthenticationTicket formsTicket = FormsAuthentication.Decrypt(Token);
                    string name = formsTicket.Name;
                    string schoolName = tblSystemLogic.getSystemValueByKey("schoolName").value;
                    if (login.IsExist(Email) && formsTicket.Name == Email && !formsTicket.Expired && formsTicket.UserData == schoolName)//take from db
                    {
                        await SignInAsync(Email);
                        return RedirectToAction("login");
                    }
                    else
                    {

                        if (login.IsExist(Email))
                        {


                            if (formsTicket.Expired && !login.checkIfregisterd(name))
                                LoginLogic.deleteByEmail(name);
                            return RedirectToAction("Error");
                            //expierd- delete data from DB - register again
                            // return to registration page

                        }
                        return RedirectToAction("Error");
                    }
                }
            }
            else
            {
                return RedirectToAction("error");
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

    }
}

