using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.MessagesModule {
    public interface IMessagesModuleEntity {
        int Id { get; }
    }

    public interface IWildcard {
        /// <summary>
        /// returns enumerable where: key - is wildcard from template, 
        /// value - name of column from database table
        /// </summary>
        IEnumerable< KeyValuePair<string, string> > ToKeyValues();
    }

    public partial class tblFilter: IMessagesModuleEntity {

    }
    public partial class tblRecepientFilter: IMessagesModuleEntity {
    }
    public partial class tblRecepientFilterTableName: IMessagesModuleEntity {
        //TODO should have schema field!
    }
    public partial class tblTemplate: IMessagesModuleEntity {
    }

    public partial class tblWildcard: IMessagesModuleEntity, IWildcard {

        public IEnumerable<KeyValuePair<string, string>> ToKeyValues() {
            return new[] { new KeyValuePair<string,string>(Code,Key) };
        }
    }

    public partial class tblRecepientCard : IMessagesModuleEntity, IWildcard {
        public const string NameCode = "{REC_NAME}";
        public const string PhoneCode = "{REC_PHONE}";
        public const string EmailCode = "{REC_EMAIL}";

        public IEnumerable<KeyValuePair<string, string>> ToKeyValues() {
            return new[] {
                new KeyValuePair<string, string>(NameCode, NameKey),
                new KeyValuePair<string, string>(PhoneCode, PhoneKey),
                new KeyValuePair<string, string>(EmailCode, EmailKey),
            };
        }
    }
}