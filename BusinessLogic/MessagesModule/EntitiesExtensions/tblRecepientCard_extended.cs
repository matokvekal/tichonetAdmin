using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.EntitiesExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule {
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
