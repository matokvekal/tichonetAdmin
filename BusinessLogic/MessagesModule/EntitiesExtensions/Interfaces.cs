using Business_Logic.MessagesModule.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.EntitiesExtensions {

    public interface IErrorLoged {
        void AddError(string errorMessage);
    }

    public interface IMessage: IErrorLoged {
        
        DateTime? SendDate { get; set; }
    }

    public interface IEmailMessage : IMessage {
        string RecepientName { get; }
        string RecepientAdress { get; }
        string Subject { get; }
        string Body { get; }
        bool IsBodyHtml { get; }
    }

    public interface ISendServiceProvider {
        SendProviderRestrictionData RestrictionData { get; }
        SendProviderRestrictionDataLog RestrictionDataLog { get; set; }
    }
    public interface IEmailServiceProvider : ISendServiceProvider {
        string SmtpHostName { get; }
        int SmtpPort { get; }
        bool EnableSsl { get; }

        MailAddress FromEmailAddress { get; }
        NetworkCredential NetworkCredentials { get;}
    }

    public interface IMessagesModuleEntity {
        int Id { get; }
    }

    public interface IWildcard {
        /// <summary>
        /// returns enumerable where: key - is wildcard from template, 
        /// value - name of column from database table
        /// </summary>
        IEnumerable<KeyValuePair<string, string>> ToKeyValues();
    }

    public interface IMessageTemplate {
        string MsgHeader { get; }
        string MsgBody { get; }
        string TableWithKeysName { get; }
        IQueryable<tblFilter> Filters { get; }
        IEnumerable<FilterValueContainer> FilterValueContainers { get; }
        IQueryable<tblWildcard> Wildcards { get; }
        IQueryable<tblRecepientCard> Recepients { get; }
    }
}
