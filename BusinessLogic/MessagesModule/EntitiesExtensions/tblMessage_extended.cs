using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.EntitiesExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule {
    public partial class tblMessage : IMessagesModuleEntity, IEmailMessage {
        string IEmailMessage.Body {
            get {
                return Body;
            }
        }

        bool IEmailMessage.IsBodyHtml {
            get {
                return false;
            }
        }

        string IEmailMessage.RecepientAdress {
            get {
                return Adress;
            }
        }

        string IEmailMessage.RecepientName {
            get {
                //TODO RecepientName
                return Adress;
            }
        }

        DateTime? IMessage.SendDate {
            get {
                return SentOn;
            }

            set {
                SentOn = value;
            }
        }

        string IEmailMessage.Subject {
            get {
                return Header;
            }
        }

        void IErrorLoged.AddError(string errorMessage) {
            ErrorLog = string.IsNullOrEmpty(ErrorLog) ? errorMessage : ErrorLog + "; " + errorMessage;
        }
    }
}
