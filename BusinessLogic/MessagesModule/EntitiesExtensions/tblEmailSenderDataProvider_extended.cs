using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.EntitiesExtensions;
using Newtonsoft.Json;

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

        bool IEmailServiceProvider.EnableSsl { get { return EnableSsl; } }
        string IEmailServiceProvider.SmtpHostName { get { return SmtpHostName; } }
        int IEmailServiceProvider.SmtpPort { get { return SmtpPort; } }

        SendProviderRestrictionData _RestrictionData;
        SendProviderRestrictionData ISendServiceProvider.RestrictionData {
            get {
                if (_RestrictionData == null) {
                    if (string.IsNullOrWhiteSpace(SendProviderRestrictionDataJSON))
                        _RestrictionData = new SendProviderRestrictionData();
                    else
                        _RestrictionData = JsonConvert
                            .DeserializeObject<SendProviderRestrictionData>(SendProviderRestrictionDataJSON);
                }
                return _RestrictionData;
            }
        }

        SendProviderRestrictionDataLog _RestrictionDataLog;
        SendProviderRestrictionDataLog ISendServiceProvider.RestrictionDataLog {
            get {
                if (_RestrictionDataLog == null) {
                    if (string.IsNullOrWhiteSpace(SendProviderRestrictionDataLogJSON))
                        _RestrictionDataLog = new SendProviderRestrictionDataLog();
                    else
                        _RestrictionDataLog = JsonConvert
                            .DeserializeObject<SendProviderRestrictionDataLog>(SendProviderRestrictionDataLogJSON);
                }
                return _RestrictionDataLog;
            }
            set {
                SendProviderRestrictionDataLogJSON = JsonConvert.SerializeObject(value);
            }
        }

        #endregion
    }
}
