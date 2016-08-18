using System.Collections.Generic;

namespace Business_Logic.MessagesContext {
    public interface IMessagesContextEntity {
        int Id { get; }
    }

    public interface IWildcard {
        string Apply(string toString, IDictionary<string, object> data);
    }

    public partial class tblFilter: IMessagesContextEntity {
    }
    public partial class tblRecepientFilter: IMessagesContextEntity {
    }
    public partial class tblRecepientFilterTableName: IMessagesContextEntity {
        //TODO should have schema field!
    }
    public partial class tblTemplate: IMessagesContextEntity {
    }

    public partial class tblWildcard: IMessagesContextEntity, IWildcard {

        public string Apply (string toString, IDictionary<string,object> data) {
            return toString.Replace(Code, data[Key].ToString());
        }

    }

    public partial class tblRecepientCard : IMessagesContextEntity, IWildcard {
        public const string NameCode = "{REC_NAME}";
        public const string PhoneCode = "{REC_PHONE}";
        public const string EmailCode = "{REC_EMAIL}";

        public string Apply(string toString, IDictionary<string, object> data) {
            return toString
                .Replace(NameCode, data[NameKey].ToString())
                .Replace(EmailCode, data[EmailKey].ToString())
                .Replace(PhoneCode, data[PhoneKey].ToString());

        }
    }
}