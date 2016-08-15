using System.Collections.Generic;

namespace Business_Logic.MessagesContext {
    public interface IMessagesContextEntity {
        int Id { get; }
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
    public partial class tblWildcard: IMessagesContextEntity {

        public string Apply (string toString, IDictionary<string,object> data) {
            return toString.Replace(Code, data[Key].ToString());
        }

    }
}