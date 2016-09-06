using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Business_Logic.MessagesModule.EntitiesExtensions;


namespace Business_Logic.MessagesModule {
    public partial class tblEmailSenderDataProvider : IMessagesModuleEntity, IEmailServiceProvider {
        
        #region IEmailServiceProvider 

        MailAddress _FromEmailAddress;
        MailAddress IEmailServiceProvider.FromEmailAddress {
            get {
                if (_FromEmailAddress == null)
                    _FromEmailAddress = new MailAddress(FromEmailAddress, FromEmailDisplayName);
                return _FromEmailAddress;
            }
        }

        NetworkCredential _NetworkCredentials;
        NetworkCredential IEmailServiceProvider.NetworkCredentials {
            get {
                if(_NetworkCredentials == null)
                    _NetworkCredentials = new NetworkCredential(FromEmailAddress, FromEmailPassword);
                return _NetworkCredentials;
            }
        }

        int IEmailServiceProvider.MaxMsgsInHour {get {return MaxMessagesInHour; } }
        bool IEmailServiceProvider.EnableSsl { get { return EnableSsl; } }
        string IEmailServiceProvider.SmtpHostName { get { return SmtpHostName; } }
        int IEmailServiceProvider.SmtpPort { get { return SmtpPort; } }

        #endregion
    }
}
