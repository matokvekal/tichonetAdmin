using Business_Logic.SqlContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.DataObjects {
    public class ValueOperatorPair {
        public object Value { get; set; }
        public string Operator { get; set; }

        public ValueOperatorPair(string value, string oper, string sqltype) {
            Operator = oper;
            //TODO EXCEPTION HANDLE
            Value = SqlType.SqlStoreFormatToNetObject(sqltype, value);
        }

    }
}
