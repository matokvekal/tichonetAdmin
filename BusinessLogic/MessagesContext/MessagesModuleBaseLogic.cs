using System;

namespace Business_Logic.MessagesContext {

    public interface IQueryFilter {
        string key { get; set; }
        object val { get; set; }
        string op { get; set; }
    }

    public abstract class MessagesModuleBaseLogic : IDisposable {
        public MessagesModuleBaseLogic() {
            _db = new MessageContext();
        }
        public MessagesModuleBaseLogic(MessageContext openedContext) {
            _db = openedContext;
        }
        private MessageContext _db;

        protected MessageContext DB {get {return _db;}}

        public void Dispose() {_db.Dispose();}
    }
}
