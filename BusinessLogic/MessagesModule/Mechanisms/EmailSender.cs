using System.Net;
using System.Net.Mail;
using System.Web.Configuration;

namespace Business_Logic.MessagesModule.Mechanisms {

    public class EmailSender{
        public static string SystemEmailAddress
        {
            get { return WebConfigurationManager.AppSettings["SystemEmailAddress"]; }
        }
        public static string SystemEmailDisplayName
        {
            get { return WebConfigurationManager.AppSettings["SystemEmailDisplayName"]; }
        }
        public static string SystemEmailPassword
        {
            get { return WebConfigurationManager.AppSettings["SystemEmailPassword"]; }
        }
        public static string SmtpHostName
        {
            get { return WebConfigurationManager.AppSettings["SmtpHostName"]; }
        }
        public static int SmtpPort
        {
            get {
                int smtpPort = 25;
                int.TryParse(WebConfigurationManager.AppSettings["SmtpPort"], out smtpPort);
                return smtpPort;
            }
        }

        public void Send(string toEmail, string toName, string subject, string body)
        {
            var fromAddress = new MailAddress(SystemEmailAddress, SystemEmailDisplayName);
            var toAddress = new MailAddress(toEmail, toName);
            var fromPassword = SystemEmailPassword;

            using (var smtp = new SmtpClient {
                    Host = SmtpHostName,
                    Port = SmtpPort,
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                })
            {
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false // still text
                })
                {
                    smtp.Send(message);
                }
            }
        }
    }
}