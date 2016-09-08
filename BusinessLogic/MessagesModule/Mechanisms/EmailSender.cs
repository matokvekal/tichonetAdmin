using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Collections.Generic;
using Business_Logic.MessagesModule.EntitiesExtensions;
using System.Linq;
using System;

namespace Business_Logic.MessagesModule.Mechanisms {

    public class EmailSender : BatchSendingComponent {

        public EmailSender(BatchSendingManager manager) : base(manager) {
        }

        public void SendBatch (IEnumerable<IEmailMessage> messages, IEmailServiceProvider provider) {
            OpenSmptAndDO(provider, smtp => {
                foreach (var msg in messages)
                    SendEmail(msg, provider, smtp);
            });
        }

        public void SendSingle (IEmailMessage msg, IEmailServiceProvider provider) {
            OpenSmptAndDO(provider, smtp => SendEmail(msg, provider, smtp));
        }

        void SendEmail(IEmailMessage msg, IEmailServiceProvider provider, SmtpClient smtp) {

            var toAddress = new MailAddress("seme@stexsy.com", msg.RecepientName);
            //var toAddress = new MailAddress(msg.RecepientAdress, msg.RecepientName);

            using (var message = new MailMessage(provider.FromEmailAddress, toAddress) {
                Subject = msg.Subject,
                Body = msg.Body,
                IsBodyHtml = msg.IsBodyHtml // still text
            })
                try {
                    smtp.Send(message);
                    msg.SendDate = DateTime.Now;
                }
                catch (SmtpException e) {
                    msg.AddError("SmtpException: " + e.Message); 
                }
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