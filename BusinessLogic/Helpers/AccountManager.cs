using System;
using System.Web;
using Business_Logic.Constants;
using Business_Logic.Entities;
using Newtonsoft.Json;

namespace Business_Logic.Helpers
{
    public static class AccountManager
    {
        #region Properties

        public static LoginInfo LoginInfo
        {
            get { return GetLoginInfoFromCookie(); }
            set { UpdateInfoDetails(value); }
        }

        #endregion

        #region Private Methods

        private static LoginInfo GetLoginInfoFromCookie()
        {
            try
            {
                var cookie = HttpContext.Current.Request.Cookies[GeneralConstants.LoggedAdmin];

                if (cookie != null)
                {
                    var encryptedPropertyKey = StringCipher.Encrypt(GeneralConstants.LoginInfo).Replace("=", "");

                    var loginResult = JsonConvert.DeserializeObject(StringCipher.Decrypt(cookie.Values[encryptedPropertyKey]), typeof(LoginInfo)) as LoginInfo;

                    return loginResult;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private static void UpdateInfoDetails(LoginInfo login)
        {
            try
            {
                if (login == null)
                {
                    var emptyCookie = new HttpCookie(GeneralConstants.LoggedAdmin);

                    emptyCookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(emptyCookie);

                    return;
                }

                //todo remove after
                login.InterfaceLanguage = "en";

                var cookie = new HttpCookie(GeneralConstants.LoggedAdmin);

                var loginJson = JsonConvert.SerializeObject(login);

                var encryptedPropertyKey = StringCipher.Encrypt(GeneralConstants.LoginInfo).Replace("=", "");
                var encryptedPropertyValue = StringCipher.Encrypt(loginJson);

                cookie.Values.Add(encryptedPropertyKey, encryptedPropertyValue);

                if (login.UserRole == Enums.UserRole.Student)
                    cookie.Expires = DateTime.Now.AddDays(365*10);
                else
                    cookie.Expires = DateTime.Now.AddDays(5);

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            catch (Exception ex)
            {
                //Logger.Log(ex);
            }
        }

        #endregion
    }
}
