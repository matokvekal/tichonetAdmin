using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Collections.Generic;
using Business_Logic.MessagesModule.EntitiesExtensions;
using System.Linq;
using System;

namespace Business_Logic.MessagesModule.Mechanisms {

    public class EmailSender {

        public void SendSingle (IEmailMessage msg, IEmailServiceProvider provider) {
            OpenSmptAndDO(provider, smtp => SendEmail(msg, provider, smtp));
        }

        void SendEmail(IEmailMessage msg, IEmailServiceProvider provider, SmtpClient smtp) {
            //TODO CATCH SmtpException HERE
            var toAddress = new MailAddress(msg.RecepientAdress, msg.RecepientName);

            using (var message = new MailMessage(provider.FromEmailAddress, toAddress) {
                Subject = msg.Subject,
                Body = msg.Body,
                IsBodyHtml = msg.IsBodyHtml // still text
            })
                smtp.Send(message);
        }

        void OpenSmptAndDO(IEmailServiceProvider provider, Action<SmtpClient> action) {
            using (var smtp = new SmtpClient {
                Host = provider.SmtpHostName,
                Port = provider.SmtpPort,
                EnableSsl = provider.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = provider.NetworkCredentials
            })
                action(smtp);
        }
    }
}